namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCompanyDailyWinlossCriteria : CriteriaBase
    {
        public GetCompanyDailyWinlossCriteria(DateTime winLossDate, int? custId, string groupName, DateTime moveBetsDate)
        {
            this.WinLossDate = winLossDate;
            this.CustId = custId > 0 ? custId : null;
            this.GroupName = groupName;
            this.MoveBetsDate = moveBetsDate;
        }
        [SpParam(SqlParamName = "WinlostDate")]
        public DateTime WinLossDate { get; }

        public int? CustId { get; }

        public string GroupName { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinLossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_Com" : "EAS_Rpt_WinLossDaily_Com14";
    }
}
