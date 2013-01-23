namespace Fanex.ExternalAccount.DataAccess.Impl.Statement
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.Common.Extensions;
    using Fanex.ExternalAccount.DataAccess.Impl.Base;
    using Fanex.ExternalAccount.DataAccess.Interface.Statement;
    using Fanex.ExternalAccount.DTO.Statement;

    public class StatementDAO : DAOBase, IStatementDAO
    {
        public List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDay, DateTime moveBetDay, bool includeAccBal)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("custid", custId);
            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);
            parameters.Add("movebetday", moveBetDay);
            parameters.Add("includeaccbal", includeAccBal);

            var result = new List<Statement>();

            using (IDataReader reader = ExecDataReader("Acc_AllStatement_Full", parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        Statement entity = new Statement
                        {
                            AccDate = reader.ToDateTime("AccDate"),
                            RefNo = reader.ToInt("RefNo"),
                            TotalAmt = reader.ToDouble("TotalAmt"),
                            TransType = reader.ToString("TransType"),
                            TransDesc = reader.ToString("TransDesc")
                        };
                        result.Add(entity);
                    }
                }
            }

            return result;
        }
    }
}