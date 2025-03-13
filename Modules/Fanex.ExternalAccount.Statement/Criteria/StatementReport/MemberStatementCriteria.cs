namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using Fanex.ExternalAccount.Statement.Services;
    //using Fanex.NAP.Framework.Data;
    using Fanex.Data;
    public class MemberStatementCriteria : StatementReportCriteria
    {
        [SpParam(Ignored = true)]
        public int RoleId { get; set; }

        [SpParam(Ignored = true)]
        public string Language
        {
            get
            {
                return "EN";
            }
        }

        [SpParam(Ignored = true)]
        public int PageSize { get; set; }

        [SpParam(Ignored = true)]
        public int PageIndex { get; set; }

        public override string GetSettingKey()
        {
            return "Statement_Report_GetList" + ReportService.GetStoreMappingSuffix(this);
        }
    }
}
