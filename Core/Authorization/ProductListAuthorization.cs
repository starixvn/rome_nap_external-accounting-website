namespace Fanex.ExternalAccount.Core.Authorization
{
    using System;
    using System.Linq;

    using System.Collections.Generic;
    using Context;
    using CustomerSetting.Module.Core.Criteria;
    using CustomerSetting.Module.Core.Models;
    using CustomerSetting.Module.Core.Services.Impl;
    using CustomerSetting.Module.Core.Services.Interface;
    using CustomerSetting.Module.Core.Web;
    using DataAccess.Criteria.GroupConfiguration;
    using DTO.UserControl;
    using Service.UserControl;
    using Shared;
    using Utilities.Reports.WinLossDetail;

    public class ProductListAuthorization
    {
        private readonly ICurrentSession currentSession;
        private readonly IServiceBase permissionService;
        private readonly IGroupConfigurationService groupConfigurationService;

        public ProductListAuthorization() : this(null, null, null)
        { }

        public ProductListAuthorization(
            ICurrentSession currentSession,
            IServiceBase permissionService,
            IGroupConfigurationService groupConfigurationService)
        {
            this.currentSession = currentSession ?? Context.CurrentSession.Instance;
            this.permissionService = permissionService ?? new ServiceBase();
            this.groupConfigurationService = groupConfigurationService ?? new GroupConfigurationService();
        }

        public IEnumerable<GroupConfigurationModel> GetProductList(bool isHideInactiveProduct)
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
                    if ((isHideInactiveProduct && product.IsHideInactiveProduct)
                        || (product.IsSpecial
                        && AdminDoesNotHavePermissionOnSpecialProduct(checkIsInternalAdminResult, isAuthorized)))
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
    }
}
