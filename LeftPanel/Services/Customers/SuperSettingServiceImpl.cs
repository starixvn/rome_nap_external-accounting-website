namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public class SuperSettingServiceImpl : CustomerSettingServiceImpl
    {
        private readonly AuthorizationService authorizationService;

        private readonly IReadOnlyDictionary<int, string> visibleBetSettingPermissions;

        public SuperSettingServiceImpl(
            int userId,
            int roleId,
            AuthorizationService authorizationService = null,
            IReadOnlyDictionary<int, string> visibleBetSettingPermissions = null)
            : base(userId, roleId)
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
#pragma warning disable S109 // Magic numbers should not be used
                setting.Details.Insert(2, new ObjectInfo("BetSetting", "Betting Limit"));
#pragma warning restore S109 // Magic numbers should not be used
            }

            setting.Details.Add(new ObjectInfo("DataRange", "Data Range"));
            return setting;
        }
    }
}