namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using Fanex.NAP.Framework.Data;

    public class StatementReportCriteria : DateRangeReportCriteria
    {
        public int CustId { get; set; }

        [SpParam(Ignored = true)]
        public string Username { get; set; }
    }
}
