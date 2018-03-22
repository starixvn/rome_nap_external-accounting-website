namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using ViewModels;

    public class SuperSettingServiceImpl : CustomerSettingServiceImpl
    {
        private readonly AuthorizationService authorizationService;
        private readonly IReadOnlyDictionary<int, string> visibleBetSettingPermissions;

        public SuperSettingServiceImpl(
            AuthorizationService authorizationService = null,
            CustomerStatusQueryService customerStatusService = null,
            IReadOnlyDictionary<int, string> visibleBetSettingPermissions = null) : base(customerStatusService)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.visibleBetSettingPermissions = visibleBetSettingPermissions ?? AccountInfoProductConfiguration.VisibleBetSettingPermissions;
        }

        public override Setting GetSetting(
            int actorId,
            string custName,
            int uplineId,
            string groupName,
            DataSet customerInfoDataset)
        {
            var setting = base.GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);

            if (authorizationService.HasReadPermissionOnFunctions(
                visibleBetSettingPermissions.Values, actorId))
            {
                setting.Details.Insert(2, new ObjectInfo("BetSetting", "Betting Limit"));
            }

            return setting;
        }
    }
}