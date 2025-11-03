namespace Fanex.ExternalAccount.Statement.Services.StatementReport
{
    using System.IO;
    using Fanex.ExternalAccount.Statement.Criteria;
    using Fanex.ExternalAccount.Statement.Criteria.StatementReport;
    using Fanex.ExternalAccount.Statement.Models;
    using Fanex.ExternalAccount.Statement.Models.StatementReport;

    public interface IStatementReportService
    {
        StatementReportModel SMAStatement(SMAStatementCriteria criteria);

        StatementReportModel MemberStatement(MemberStatementCriteria criteria);

        StatementReportModel TransferDetails(TransferDetailsCriteria criteria);

        MemoryStream ExportMemberStatement(MemberStatementCriteria criteria, StatementReportModel data);

        MemoryStream ExportSMAStatement(SMAStatementCriteria criteria, StatementReportModel data);

        MemoryStream ExportTransferDetails(TransferDetailsCriteria criteria, StatementReportModel data);

        CustomerInformationModel GetCustomerInfo(GetCustomerInformationByUsername criteria);
        
    }
}
