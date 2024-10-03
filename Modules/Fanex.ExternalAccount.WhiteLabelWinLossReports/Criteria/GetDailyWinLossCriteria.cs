namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Criteria
{
    using System;

    public class GetDailyWinLossCriteria : GetSettlementCriteria
    {
        public GetDailyWinLossCriteria(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            string whiteLabels,
            string username) : base(groupName, fromDate, toDate, whiteLabels, username)
        {
        }

        public override string GetSettingKey() => "EAS_Rpt_WhiteLabelWinLoss";
    }
}