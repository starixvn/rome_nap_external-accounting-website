// -----------------------------------------------------------------------
// <copyright file="StatementService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Statement
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DataAccess.Impl.Statement;
    using Fanex.ExternalAccount.DataAccess.Interface.Statement;
    using Fanex.ExternalAccount.DTO.Statement;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Statement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class StatementService : ServiceBase, IStatementService
    {
        private const string BALANCEUP_DAY_KEY = "SUNPLUS.AGENT.MODEL.BL.STATEMENT.BALANCEUPDAY";
        private readonly IStatementDAO StatementDAO = null;

        public StatementService(IStatementDAO StatementDAO = null)
        {
            this.StatementDAO = StatementDAO ?? new StatementDAO();
        }
        
        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            bool includeAccBal = false;
            if (fromDate != ReportService.BalanceUpDate)
            {
                includeAccBal = true;
            }
            return StatementDAO.Acc_AllStatement_Full(custId, fromDate, toDate, ReportService.BalanceUpDate, ReportService.MoveBetsDate, includeAccBal);
        }

    }
}
