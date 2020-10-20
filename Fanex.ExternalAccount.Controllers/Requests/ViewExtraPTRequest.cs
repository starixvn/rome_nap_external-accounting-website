namespace Fanex.ExternalAccount.Controllers.Requests
{
    using Core.Context;
    using Core.Reports;
    using DataAccess.Criteria.GroupConfiguration;
    using Fanex.ExternalAccount.Core.Authorization;
    using Service.Interface.Report;
    using Service.UserControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public class ViewExtraPTRequest : IViewExtraPTRequest
    {
        private readonly Func<int, ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter;
        private readonly IExtRequest extRequest;
        private readonly IGroupConfigurationService groupConfigurationService;

        public ViewExtraPTRequest() : this(null, null, null)
        {
        }

        public ViewExtraPTRequest(
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

        public int[] GetSelectedProductIds()
        {
            var filterParams = selectedProductQueryGetter(GroupId);
            var products = groupConfigurationService.Get(new GetGroupConfigurationCriteria()).Where(model => model.GroupName == GroupName);

            return products
                .Where(product => product.ProductId != 0 && filterParams.Any(filter => filter.Key.Equals(product.Id, StringComparison.OrdinalIgnoreCase) && filter.Value))
                .Select(product => product.ProductId).ToArray();
        }

        public string GetProductName(int productId)
        {
            return new ProductListAuthorization()
                .GetProductList(GroupId)
                .SingleOrDefault(p => p.ProductId == productId)?
                .ProductText;
        }
    }
}