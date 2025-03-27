namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMasterDailyWinlossCriteria : CriteriaBase
    {
        public GetMasterDailyWinlossCriteria(DateTime winLossDate, int custId, int? agentId, DateTime moveBetsDate)
        {
            this.WinLossDate = winLossDate;
            this.CustId = custId;
            this.AgentId = agentId > 0 ? agentId : null;
            this.MoveBetsDate = moveBetsDate;
        }
        
        [SpParam(SqlParamName = "WinlostDate")]
        public DateTime WinLossDate { get; }

        public int CustId { get; }

        public int? AgentId { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinLossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_MAg" : "EAS_Rpt_WinLossDaily_MAg14";
    }
}
