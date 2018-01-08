namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Core.Shared;
    using Extensions;
    using ViewModels;

    public class MasterInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        internal override BalanceInfo GetBalanceInfo(int userId, DataRow customerRow, DataRow cashRow)
        {
            var balanceInfo = base.GetBalanceInfo(userId, customerRow, cashRow);

            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalMasterBalance(),
                Text = "Total Balance"
            });
            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalMasterYesterdayCash(),
                Text = "Yesterday Balance"
            });
            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Agent Outstanding"
            });

            return balanceInfo;
        }

        internal override void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.UplineId = new ObjectInfo { Value = lineRow.SuperId(), Text = "Super ID" };
            personalInfo.CurrentLevel = new ObjectInfo { Value = lineRow.MasterName(), Text = "Master" };
            personalInfo.Downlines = GetDownlines(lineRow, Role.Master);
        }

        internal override void SetCustomSettingByLevel(Setting setting, int userId, DataSet dataSet)
        {
            var cashRow = dataSet.Tables[2].Rows[0];
            setting.EnableItems.Insert(2, new ObjectInfo("AutoPT", cashRow.EnableAutoMasterPT(), "Allow Auto PT"));
        }
    }
}