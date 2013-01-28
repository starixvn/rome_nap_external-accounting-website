// -----------------------------------------------------------------------
// <copyright file="IStatementService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Interface.Statement
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO.Statement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IStatementService
    {
        List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate);
    }
}