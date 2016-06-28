// --------------------------------------------------------------------------
// <copyright file="StatementDAO.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

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
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Statement data access object.
    /// </summary>
    public class StatementDAO : DAOBase, IStatementDAO
    {
        /// <summary>
        /// Gets all statements.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="fromDate">From date.</param>
        /// <param name="toDate">To date time.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <param name="moveBetDay">Move bet day.</param>
        /// <param name="includeAccBal">Include account balance or not.</param>
        /// <returns>List object.</returns>
        public List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate, DateTime balanceUpDay, DateTime moveBetDay, bool includeAccBal)
        {
            Hashtable parameters = new Hashtable();
            parameters.Add("custid", custId);
            parameters.Add("fromdate", fromDate);
            parameters.Add("todate", toDate);
            parameters.Add("balanceupday", balanceUpDay);
            parameters.Add("movebetday", moveBetDay);
            parameters.Add("includeaccbal", includeAccBal);
            parameters.Add("showexchangerate", false);

            string type = string.Empty;
            string remark = string.Empty;
            var result = new List<Statement>();

            using (IDataReader reader = ExecDataReader("Acc_AllStatement_Full", parameters))
            {
                if (reader != null)
                {
                    while (reader.Read())
                    {
                        type = reader.IsDBNull(reader.GetOrdinal("TransType")) ? string.Empty : reader["TransType"].ToString().ToUpper(); 
                        remark = reader.IsDBNull(reader.GetOrdinal("TransDesc")) ? string.Empty : reader["TransDesc"].ToString();
                        bool isAccBal = string.Compare(remark.ToLowerInvariant(), "accumulated balance", true) == 0;

                        Statement entity = new Statement
                        {
                            AccDate = GetDate(type, isAccBal, Convert.ToDateTime(reader["AccDate"]), balanceUpDay),
                            RefNo = reader.ToLong("RefNo"),
                            TotalAmt = Convert.ToDouble(reader["TotalAmt"]),
                            TransType = type,
                            TransDesc = remark
                        };
                        result.Add(entity);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Get date time.
        /// </summary>
        /// <param name="transType">Transaction type.</param>
        /// <param name="isAccBal">Is account balance or not.</param>
        /// <param name="accDate">Account date.</param>
        /// <param name="balanceUpDay">Balance up day.</param>
        /// <returns>String object.</returns>
        private string GetDate(string transType, bool isAccBal, DateTime accDate, DateTime balanceUpDay)
        {
            if (string.Compare(transType.ToLowerInvariant(), "bet", true) == 0 && accDate >= balanceUpDay)
            {
                return accDate.DayOfWeek + "<br/>" + Utils.FormatDateOptionalTime(accDate);
            }
            else
            {
                // Do not show date at ACCUMULATED BALANCE row
                if (isAccBal)
                {
                    return "--";
                }
                else
                {
                    return accDate.DayOfWeek + " <br/>" + Utils.FormatDateOptionalTime(accDate);
                }
            }
        }
    }
}