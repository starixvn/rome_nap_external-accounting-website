namespace Fanex.ExternalAccount.Service.Impl.Statement
{
    using System;
    using System.Collections.Generic;
    using Base;
    using DataAccess.Impl.Statement;
    using DataAccess.Interface.Statement;
    using DTO.Statement;
    using Interface.Statement;
    using Report;

    public class StatementService : ServiceBase, IStatementService
    {
        private readonly IStatementDao _statementDAO;

        public StatementService(IStatementDao statementDAO = null)
        {
            _statementDAO = statementDAO ?? new StatementDAO();
        }

        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            var includeAccBal = fromDate != ReportService.BalanceUpDate;

            return _statementDAO.Acc_AllStatement_Full(custId, fromDate, toDate, ReportService.BalanceUpDate,
                ReportService.MoveBetsDate, includeAccBal);
        }
    }
}