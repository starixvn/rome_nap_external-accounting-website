namespace Fanex.ExternalAccount.Core.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using CustomerSetting.Module.Core.Web;
    using DTO.UserControl;
    using RemotingRef;
    using Reports.WinLossDetail;

    public class ProductListAuthorization
    {
        private readonly HttpRequest request;
        private readonly ICurrentSession currentSession;
        private readonly IServiceBase permissionService;

        public ProductListAuthorization() : this(null, null, null, null)
        { }

        public ProductListAuthorization(
            HttpRequest request,
            ICurrentSession currentSession,
            IServiceBase permissionService,
            IRemotingRefService remotingRefService)
        {
            this.request = request ?? HttpContext.Current.Request;
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

            var products = ProductsConfiguration.Instance.ProductCollection;

            foreach (ProductElementConfiguration product in products)
            {
                if (!IsHiddenProduct(product))
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

        private bool IsHiddenProduct(ProductElementConfiguration product)
        {
            if (product.Hide)
            {
                return true;
            }

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var requestUrl = request != null && request.UrlReferrer != null
                ? request.UrlReferrer.AbsolutePath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries)
                : new string[0];
            var groupName = requestUrl.Length == 0 || !currentSession.IsInternalAdmin
                ? currentSession.CurrencyGroup
                : requestUrl[requestUrl.Length - 1];

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
