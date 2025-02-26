namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetMasterDailyWinlossCriteria : CriteriaBase
    {
        public GetMasterDailyWinlossCriteria(DateTime WinlossDate, int CustId, int AgentId, DateTime MoveBetsDate)
        {
            this.WinlossDate = WinlossDate;
            this.CustId = CustId;
            this.AgentId = AgentId;
            this.MoveBetsDate = MoveBetsDate;
        }

        public DateTime WinlossDate { get; }

        public int CustId { get; }

        public int AgentId { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinlossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_MAg" : "EAS_Rpt_WinLossDaily_MAg14";
    }
}
