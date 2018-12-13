namespace Fanex.ExternalAccount.Core.Authorization
{
    using System;
    using System.Collections.Generic;
    using Context;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using CustomerSetting.Module.Core.Web;
    using DTO.UserControl;
    using Reports.WinLossDetail;

    public class ProductListAuthorization
    {
        private readonly ExtRequest extRequest;
        private readonly ICurrentSession currentSession;
        private readonly IServiceBase permissionService;

        public ProductListAuthorization() : this(null, null, null)
        { }

        public ProductListAuthorization(
            ExtRequest extRequest,
            ICurrentSession currentSession,
            IServiceBase permissionService)
        {
            this.extRequest = extRequest ?? new ExtRequest();
            this.currentSession = currentSession ?? Context.CurrentSession.Instance;
            this.permissionService = permissionService ?? new ServiceBase();
        }

        public IEnumerable<ProductElementConfiguration> GetProductList(bool isHideInactiveProduct)
        {
            var custId = currentSession.CustId;

            var checkIsInternalAdminResult =
                permissionService.CheckCustomerIsInternalAdmin(
                    new CheckCustomerIsInternalAdminCriteria { CustID = custId });

            var isAuthorized = permissionService.CheckCustomerHasSpecialSitePermission(
                new CheckCustomerHasSpecialSiteFunctionPermissionCriteria { CustID = custId });

            var groupName = extRequest.GetGroupName();
            var products = ProductsConfiguration.Instance.ProductCollection;

            foreach (ProductElementConfiguration product in products)
            {
                if (!IsHiddenProduct(product, groupName))
                {
                    if ((isHideInactiveProduct && product.IsHideInactiveProduct)
                        || (product.IsSpecial
                        && AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized)))
                    {
                        continue;
                    }

                    yield return product;
                }
            }
        }

        private static bool AdminDoesNotHavePermissionOnSpecialProduct(
            CheckCustomerIsInternalAdminResult checkIsInternalAdminResult,
            bool isAuthorized)
            => checkIsInternalAdminResult.IsInternalAdmin
               && (!checkIsInternalAdminResult.IsAllowAccess || !isAuthorized);

        private static bool IsHiddenProduct(ProductElementConfiguration product, string groupName)
        {
            if (product.Hide)
            {
                return true;
            }

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var groups = product.ShowForGroups.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries);

            foreach (var group in groups)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase)
                    || Group.IsOfficeGroup(groupName))
                {
                    return false;
                }
            }

            return true;
        }
    }
}
