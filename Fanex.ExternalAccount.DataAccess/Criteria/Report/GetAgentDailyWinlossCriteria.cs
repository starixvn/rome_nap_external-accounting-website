namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetAgentDailyWinlossCriteria : CriteriaBase
    {
        public GetAgentDailyWinlossCriteria(DateTime WinlossDate, int CustId, int MemberId, DateTime MoveBetsDate)
        {
            this.WinlossDate = WinlossDate;
            this.CustId = CustId;
            this.MemberId = MemberId;
            this.MoveBetsDate = MoveBetsDate;
        }

        public DateTime WinlossDate { get; }

        public int CustId { get; }

        public int MemberId { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinlossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_Ag" : "EAS_Rpt_WinLossDaily_Ag14";
    }
}
