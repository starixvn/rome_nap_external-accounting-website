// -----------------------------------------------------------------------
// <copyright file="IStatementDAO.cs" company="Nexcel Solution Co.Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Interface.Statement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO.Statement;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface IStatementDAO
    {
        List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDay, DateTime moveBetDay, bool includeAccBal);
    }
}