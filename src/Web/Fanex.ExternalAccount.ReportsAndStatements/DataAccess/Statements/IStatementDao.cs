using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Statements;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.DataAccess.Statements
{
    public interface IStatementDao
    {
        List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDate, DateTime moveBetsDate, bool includeAccBal);
    }
}
