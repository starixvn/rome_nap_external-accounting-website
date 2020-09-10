namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Linq;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using CustomerSetting.Module.Core.Web;
    using DataAccess.Criteria.GroupConfiguration;
    using DTO.UserControl;
    using Fanex.ExternalAccount.DTO.Products;
    using Fanex.ExternalAccount.Service.Products;
    using Service.UserControl;
    using Utilities.Reports.WinLossDetail;

    public class ProductListAuthorization
    {
        private readonly ICurrentSession currentSession;
        private readonly IServiceBase permissionService;
        private readonly IGroupConfigurationService groupConfigurationService;
        private readonly IProductDetailService productDetailService;

        public ProductListAuthorization() : this(null, null, null, null)
        { }

        public ProductListAuthorization(
            ICurrentSession currentSession,
            IServiceBase permissionService,
            IGroupConfigurationService groupConfigurationService,
            IProductDetailService productDetailService)
        {
            this.currentSession = currentSession ?? Context.CurrentSession.Instance;
            this.permissionService = permissionService ?? new ServiceBase();
            this.groupConfigurationService = groupConfigurationService ?? new GroupConfigurationService();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public IEnumerable<GroupConfigurationModel> GetProductList()
        {
            var custId = currentSession.CustId;

            var checkIsInternalAdminResult =
                permissionService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = custId });

            var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = custId });

            var products = GetProductConfiguration().OrderBy(i => i.ProductId);
            var showedProductList = new List<GroupConfigurationModel>();
            foreach (var product in products)
            {
                if (!showedProductList.Any(i => i.ProductId == product.ProductId) && product.IsShow && !string.IsNullOrEmpty(product.Id))
                {
                    if ((product.IsSpecial && AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized)))
                    {
                        continue;
                    }

                    showedProductList.Add(product);
                }
            }
            return showedProductList;
        }

        private IEnumerable<GroupConfigurationModel> GetProductConfiguration()
        {
            if (Group.IsOfficeGroup(currentSession.CurrencyGroup))
            {
                return groupConfigurationService.Get(new GetGroupConfigurationCriteria());
            }

            return groupConfigurationService.Get(new GetGroupConfigurationCriteria { GroupId = currentSession.GroupId });
        }

        private static bool AdminDoesNotHavePermissionOnSpecialProduct(
            CheckCustomerIsInternalAdminResult checkIsInternalAdminResult,
            bool isAuthorized)
            => checkIsInternalAdminResult.IsInternalAdmin
               && (!checkIsInternalAdminResult.IsAllowAccess || !isAuthorized);

        public IEnumerable<ProductDetailDto> GetProductDetails(string groupName)
        {
            var groupConfigurationModels = GetProductList();
            var productDetails = productDetailService.GetProductDetails(groupName);

            foreach (var item in productDetails)
            {
                var group = groupConfigurationModels.FirstOrDefault(g => g.ProductId == item.ProductId);

                if (group != null)
                {
                    yield return new ProductDetailDto
                    {
                        RefId = item.RefId,
                        ResourceId = item.ResourceId,
                        ProductKey = group.Id
                    };
                }
            }
        }
    }
}
