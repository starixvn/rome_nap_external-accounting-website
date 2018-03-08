namespace Fanex.ExternalAccount.DataAccess.Interface.Statement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO.Statement;

    public interface IStatementDao
    {
        List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDay, DateTime moveBetDay, bool includeAccBal);
    }
}