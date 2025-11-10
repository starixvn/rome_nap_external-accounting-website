using System;
using System.Collections.Generic;
using Fanex.ExternalAccount.ReportsAndStatements.Models.DTOs.Statements;

namespace Fanex.ExternalAccount.ReportsAndStatements.Services.Statements
{
    public interface IStatementService
    {
        List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate);
    }
}
