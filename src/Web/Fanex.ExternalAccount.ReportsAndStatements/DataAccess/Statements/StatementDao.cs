using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Statements;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.DataAccess.Statements
{
    public class StatementDao : IStatementDao
    {
        public List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDate, DateTime moveBetsDate, bool includeAccBal)
        {
            // TODO: Implement actual database call to stored procedure Acc_AllStatement_Full
            // Parameters: custId, fromDate, toDate, balanceUpDate, moveBetsDate, includeAccBal
            return new List<Statement>();
        }
    }
}
