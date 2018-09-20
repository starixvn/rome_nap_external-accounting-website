namespace Fanex.ExternalAccount.Controllers.Requests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Core.Context;
    using Core.Reports;
    using Core.Reports.WinLossDetail;

    public class WinLossDetailRequest
    {
        private const string Seperator = ", ";
        private const string ShowAll = "showAll";
        private const string ShowPG = "showPG";

        private readonly Func<ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter;
        private readonly IExtRequest extRequest;
        public WinLossDetailRequest() : this(null, null)
        {
        }

        public WinLossDetailRequest(
            Func<ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
            IExtRequest extRequest)
        {
            this.selectedProductQueryGetter = selectedProductQueryGetter ?? new FilterController().AsCollection;
            this.extRequest = extRequest ?? new ExtRequest();
        }

        public string groupName => extRequest.GetGroupName();

        public string username { get; set; }
        public int srecommend { get; set; }
        public int mrecommend { get; set; }
        public int recommend { get; set; }
        public int custId { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public int viewlevel { get; set; }

        public bool IncludePockerGames { get; set; }

        public bool IsSearch() => custId == 0;

        internal string GetProductIds()
        {
            var filterParams = selectedProductQueryGetter();

            var formatedProductIds = new List<int>();

            foreach (ProductElementConfiguration item in ProductsConfiguration.Instance.ProductCollection)
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
            var filterParams = selectedProductQueryGetter();
            var formatedProducts = string.Empty;

            filterParams.ToList().ForEach(o =>
            {
                productList.ForEach(p =>
                {
                    if (!p.Key.Equals(ShowAll, StringComparison.OrdinalIgnoreCase) && o.Value
                        && (o.Value || (IncludePockerGames && p.Key.Equals(ShowPG, StringComparison.OrdinalIgnoreCase)))
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
