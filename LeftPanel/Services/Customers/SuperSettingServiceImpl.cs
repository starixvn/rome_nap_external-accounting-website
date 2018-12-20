namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using NAP.Framework.Data;
    using ViewModels;

    public class SuperSettingServiceImpl : CustomerSettingServiceImpl
    {
        private readonly AuthorizationService authorizationService;

        private readonly IReadOnlyDictionary<int, string> visibleBetSettingPermissions;

        public SuperSettingServiceImpl(
            int userId,
            int roleId,
            string site,
            AuthorizationService authorizationService = null,
            IReadOnlyDictionary<int, string> visibleBetSettingPermissions = null)
            : base(userId, roleId, site)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.visibleBetSettingPermissions = visibleBetSettingPermissions ?? AccountInfoProductConfiguration.VisibleBetSettingPermissions;
        }

        public override Setting GetSetting(DataSet customerStatusInfoDataset)
        {
            var setting = base.GetSetting(customerStatusInfoDataset);

            if (authorizationService.HasReadPermissionOnFunctions(
                visibleBetSettingPermissions.Values, _userId))
            {
                setting.Details.Insert(2, new ObjectInfo("BetSetting", "Betting Limit"));
            }

            setting.Details.Add(new ObjectInfo("DataRange", "Data Range"));
            return setting;
        }
    }
}