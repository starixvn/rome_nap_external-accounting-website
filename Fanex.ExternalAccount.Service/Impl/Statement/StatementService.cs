namespace Fanex.ExternalAccount.Service.Impl.Statement
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.Statement;
    using Fanex.ExternalAccount.DataAccess.Interface.Statement;
    using Fanex.ExternalAccount.DTO.Statement;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Statement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StatementService : IStatementService
    {
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.STATEMENT.BALANCEUPDAY";
        private readonly IStatementDAO _statementDAO = null;

        public StatementService(IStatementDAO statementDAO = null)
        {
            this._statementDAO = statementDAO ?? new StatementDAO();
        }
        
        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            bool includeAccBal = false;
            if (fromDate != ReportService.BalanceUpDate)
            {
                includeAccBal = true;
            }

            return _statementDAO.Acc_AllStatement_Full(custId, fromDate, toDate, ReportService.BalanceUpDate, ReportService.MoveBetsDate, includeAccBal);
        }
    }
}
