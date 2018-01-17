namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Data;
    using Core.Shared;
    using Extensions;
    using ViewModels;

    public class AgentInfoQueryServiceImpl : CustomerInfoQueryBaseService
    {
        internal override BalanceInfo GetBalanceInfo(int userId, DataRow customerRow, DataRow cashRow)
        {
            var balanceInfo = base.GetBalanceInfo(userId, customerRow, cashRow);

            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentBalance(),
                Text = "Total Balance"
            });
            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.TotalAgentYesterdayCash(),
                Text = "Yesterday Balance"
            });
            balanceInfo.Balances.Add(new ObjectInfo
            {
                Value = cashRow.CustomerOutstanding(),
                Text = "Member Outstanding"
            });

            return balanceInfo;
        }

        internal override void SetLineInfo(PersonalInfo personalInfo, DataRow lineRow)
        {
            personalInfo.UplineId = new ObjectInfo {Value = lineRow.MasterId(), Text = "Master ID"};
            personalInfo.CurrentLevel = new ObjectInfo {Value = lineRow.AgentName(), Text = "Agent"};
            personalInfo.Downlines = GetDownlines(lineRow, Role.Agent);
        }
    }
}