namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Criteria
{
    using System;
    //using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;

    public class GetSettlementCriteria : CriteriaBase
    {
        public GetSettlementCriteria(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            string whiteLabels,
            string username)
        {
            GroupName = groupName;
            FromDate = fromDate;
            ToDate = toDate;
            WhiteLabelList = whiteLabels;
            UserName = username;
        }

        public string GroupName { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string WhiteLabelList { get; set; }

        public string UserName { get; }

        public override string GetSettingKey() => "EAS_Rpt_WhiteLabelSettlement";
        
        public override bool IsValid() => !string.IsNullOrWhiteSpace(GroupName);
    }
}
