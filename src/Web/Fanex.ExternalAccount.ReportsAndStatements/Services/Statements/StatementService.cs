using System;
using System.Collections.Generic;
using Fanex.ExternalAccount.ReportsAndStatements.DataAccess.Statements;
using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Statements;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;

namespace Fanex.ExternalAccount.ReportsAndStatements.Services.Statements
{
    public class StatementService : IStatementService
    {
        private readonly IStatementDao _statementRepository;

        public StatementService(IStatementDao? statementRepository = null)
        {
            _statementRepository = statementRepository ?? new StatementDao();
        }

        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            var includeAccBal = fromDate != ReportService.BalanceUpDate;

            return _statementRepository.Acc_AllStatement_Full(custId, fromDate, toDate, ReportService.BalanceUpDate,
                ReportService.MoveBetsDate, includeAccBal);
        }
    }
}
