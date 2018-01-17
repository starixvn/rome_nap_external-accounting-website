namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Core.Shared;
    using Extensions;
    using ViewModels;

    public class SuperInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        internal override BalanceInfo GetBalanceInfo(int userId, DataRow customerRow, DataRow cashRow)
        {
            var balanceInfo = base.GetBalanceInfo(userId, customerRow, cashRow);

            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperBalance(),
                Text = "Total Balance"
            });

            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalSuperYesterdayCash(),
                Text = "Yesterday Balance"
            });

            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Master Outstanding"
            });

            return balanceInfo;
        }

        internal override void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.Downlines = GetDownlines(lineRow, Role.Super);
            personalInfo.CurrentLevel = new ObjectInfo {Value = lineRow.SuperName(), Text = "Super"};
        }

        internal override void SetCustomSettingByLevel(Setting setting, int userId, DataSet dataSet)
        {
            if (authorizationService.HasReadPermissionOnFunctions(
                LeftPanelProductIntegration.VisibleBetSettingPermissions.Values, userId))
            {
                setting.Details.Add(new ObjectInfo("BetSetting", "Edit Betting Limit"));
            }
        }
    }
}