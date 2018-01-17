namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using ViewModels;

    public class SuperInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        internal override void SetCustomSetting(Setting setting, int userId, DataSet dataSet)
        {
            if (authorizationService.HasReadPermissionOnFunctions(
                LeftPanelProductIntegration.VisibleBetSettingPermissions.Values, userId))
            {
                setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }
        }
    }
}