namespace Fanex.ExternalAccount.Statement.Repositories.StatementReport
{
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;

    public interface IStatementReportRepository
    {
        StatementReportModel SMAStatement(SMAStatementCriteria criteria);

        StatementReportModel MemberStatement(MemberStatementCriteria criteria);

        StatementReportModel TransferDetails(TransferDetailsCriteria criteria);

        CustomerInformationModel GetCustomerInfo(GetCustomerInformationByUsername criteria);
    }
}
