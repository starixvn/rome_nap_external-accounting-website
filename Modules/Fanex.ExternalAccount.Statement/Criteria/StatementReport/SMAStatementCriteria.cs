namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using Fanex.ExternalAccount.Statement.Services;
    using Fanex.Data;

    public class SMAStatementCriteria : StatementReportCriteria
    {
        [SpParam(Ignored = true)]
        public int RoleId { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public override string GetSettingKey()
        {
            return "Statement_Report_GetList" + ReportService.GetStoreMappingSuffix(this);
        }

        public override bool IsValid()
        {
            return true;
        }
    }
}
