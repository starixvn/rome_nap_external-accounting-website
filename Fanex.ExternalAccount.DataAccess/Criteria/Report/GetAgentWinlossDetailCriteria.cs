namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;
    using System;

    public class GetAgentWinlossDetailCriteria: CriteriaBase
    {
        public int CustId { get; }

        public int AgentId { get; }

        public DateTime FromDate { get; }

        public DateTime ToDate { get; }

        public string ProductIds { get; }

        public string ProductDetailsXML { get; }

        public DateTime BalanceUpDay { get; }

        public DateTime MoveBetsDay { get; }

#pragma warning disable S107 // Methods should not have too many parameters
        public GetAgentWinlossDetailCriteria(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay)
#pragma warning restore S107 // Methods should not have too many parameters
        {
            CustId = custId;
            AgentId = agentId;
            FromDate = fromDate;
            ToDate = toDate;
            ProductIds = productIds;
            ProductDetailsXML = productDetailsXML;
            BalanceUpDay = balanceUpDay;
            MoveBetsDay = moveBetsDay;
        }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_MAg";

        public override bool IsValid() => true;
    }
}
