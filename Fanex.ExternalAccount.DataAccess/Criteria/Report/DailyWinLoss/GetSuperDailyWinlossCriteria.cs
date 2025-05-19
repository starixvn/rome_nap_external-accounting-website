namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetSuperDailyWinlossCriteria : CriteriaBase
    {
        public GetSuperDailyWinlossCriteria(DateTime winLossDate, int custId, int? masterId, int? memberId, DateTime moveBetsDate)
        {
            this.WinLossDate = winLossDate;
            this.CustId = custId;
            this.MasterId = masterId > 0 ? masterId : null;
            this.MemberId = memberId > 0 ? memberId : null;
            this.MoveBetsDate = moveBetsDate;
        }
        
        [SpParam(SqlParamName = "WinlostDate")]
        public DateTime WinLossDate { get; }

        public int CustId { get; }

        public int? MasterId { get; }

        public int? MemberId { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinLossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_SAg" : "EAS_Rpt_WinLossDaily_SAg14";
    }
}
