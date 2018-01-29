namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Core.Authorization;
    using ViewModels;

    public class SuperSettingServiceImpl : CustomerSettingServiceImpl
    {
        private readonly AuthorizationService authorizationService;

        public SuperSettingServiceImpl(
            AuthorizationService authorizationService = null,
            CustomerStatusQueryService customerStatusService = null) : base(customerStatusService)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
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
                AccountInfoProductConfiguration.VisibleBetSettingPermissions.Values, actorId))
            {
                setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }

            return setting;
        }
    }
}