namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using System;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;

    public class DateRangeReportCriteria : CriteriaBase
    {
        private readonly IReportUtils _reportUtils;

        protected DateRangeReportCriteria(IReportUtils reportUtils = null) => _reportUtils = reportUtils ?? new ReportUtils();
        
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime BalanceUpDay =>  _reportUtils.BalanceUpDate;

        public DateTime MoveBetsDay =>  _reportUtils.MoveBetsDate;

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            return this.FromDate.Year >= 1753 &&
                this.FromDate.Year <= 9999 &&
                this.ToDate.Year >= 1753 &&
                this.ToDate.Year <= 9999 &&
                this.ToDate >= this.FromDate;
        }
    }
}
