namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data;
    using Fanex.Data.Repository;
    using System;

    public class GetCompanyDailyWinlossCriteria : CriteriaBase
    {
        public GetCompanyDailyWinlossCriteria(DateTime WinlossDate, int CustId, string GroupName, DateTime MoveBetsDate)
        {
            this.WinlossDate = WinlossDate;
            this.CustId = CustId;
            this.GroupName = GroupName;
            this.MoveBetsDate = MoveBetsDate;
        }

        public DateTime WinlossDate { get; }

        public int CustId { get; }

        public string GroupName { get; }

        [SpParam(Ignored = true)]
        public DateTime MoveBetsDate { get; }

        public override bool IsValid() => true;

        public override string GetSettingKey()
            => DateTime.Compare(WinlossDate, MoveBetsDate) > 0 ? "EAS_Rpt_WinLossDaily_Com" : "EAS_Rpt_WinLossDaily_Com14";
    }
}
