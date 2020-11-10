namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;
    using Core.Reports;
    using DataAccess.Criteria.GroupConfiguration;
    using Service.UserControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class WinLossDetailRequest
    {
        private const string Seperator = ", ";
        private const string ShowAll = "showAll";

        private readonly Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter;
        private readonly IExtRequest extRequest;
        private readonly IGroupConfigurationService groupConfigurationService;

        public WinLossDetailRequest() : this(null, null, null)
        {
        }

        public WinLossDetailRequest(
            Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
            IExtRequest extRequest,
            IGroupConfigurationService groupConfigurationService)
        {
            this.selectedProductQueryGetter = selectedProductQueryGetter ?? new FilterController().AsCollection;
            this.extRequest = extRequest ?? new ExtRequest();
            this.groupConfigurationService = groupConfigurationService ?? new GroupConfigurationService();
        }

        public int GroupId => extRequest.GetGroupId();
        public string GroupName => extRequest.GetGroupName();
        public string Username { get; set; }
        public int Srecommend { get; set; }
        public int Mrecommend { get; set; }
        public int Recommend { get; set; }
        public int CustId { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public int Viewlevel { get; set; }
        public string ResourceIds { get; set; }

        public bool IsSearch() => CustId == 0;

        internal string GetProductIds()
        {
            var filterParams = selectedProductQueryGetter(GroupId);

            var formatedProductIds = new List<int>();

            var products = groupConfigurationService.Get(new GetGroupConfigurationCriteria()).Where(i => i.GroupName == GroupName);
            foreach (var item in products)
            {
                foreach (var product in filterParams)
                {
                    if (item.ProductId != 0 && product.Key.Equals(item.Id, StringComparison.OrdinalIgnoreCase)
                        && product.Value)
                    {
                        formatedProductIds.Add(item.ProductId);
                    }
                }
            }

            return string.Join(",", formatedProductIds);
        }

        internal string GetProductNames(List<KeyValuePair<string, string>> productList)
        {
            var filterParams = selectedProductQueryGetter(GroupId);
            var formatedProducts = string.Empty;

            filterParams.ToList().ForEach(o =>
            {
                productList.ForEach(p =>
                {
                    if (!p.Key.Equals(ShowAll, StringComparison.OrdinalIgnoreCase)
                        && o.Value
                        && o.Key.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        formatedProducts += p.Value + Seperator;
                    }
                });
            });

            if (formatedProducts.Length > 0)
            {
                formatedProducts = formatedProducts.TrimEnd(Seperator.ToCharArray());
            }

            return formatedProducts;
        }
    }

}
