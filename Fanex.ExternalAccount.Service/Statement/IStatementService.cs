namespace Fanex.ExternalAccount.Service.Interface.Statement
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO.Statement;

    public interface IStatementService
    {
        List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate);
    }
}