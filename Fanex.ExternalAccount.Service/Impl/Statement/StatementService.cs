namespace Fanex.ExternalAccount.Service.Impl.Statement
{
    using System;
    using System.Collections.Generic;
    using DataAccess.Impl.Statement;
    using DataAccess.Interface.Statement;
    using DTO.Statement;
    using Interface.Report;
    using Interface.Statement;
    using Report;

    public class StatementService : IStatementService
    {
        private readonly IStatementDao StatementRepository;
        private readonly IReportService _reportService;

        public StatementService(IStatementDao statementRepository = null, IReportService reportService = null)
        {
            StatementRepository = statementRepository ?? new StatementDAO();
            _reportService = reportService ?? new ReportService();
        }

        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            var includeAccBal = fromDate != _reportService.BalanceUpDate();

            return StatementRepository.Acc_AllStatement_Full(custId, fromDate, toDate, _reportService.BalanceUpDate(),
                _reportService.MoveBetsDate(), includeAccBal);
        }
    }
}