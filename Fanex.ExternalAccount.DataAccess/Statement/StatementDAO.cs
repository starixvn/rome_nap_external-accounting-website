// namespace Fanex.ExternalAccount.DataAccess.Impl.Statement
// {
//     using System;
//     using System.Collections;
//     using System.Collections.Generic;
//     using System.Globalization;
//     using Base;
//     using Common.Extensions;
//     using DTO.Statement;
//     using Interface.Statement;
//     using Utilities;
//
//     public class StatementDao : DAOBase, IStatementDao
//     {
//         public List<Statement> Acc_AllStatement_Full(int custId, DateTime fromDate, DateTime toDate,
//             DateTime balanceUpDay, DateTime moveBetDay, bool includeAccBal)
//         {
//             var parameters = new Hashtable
//             {
//                 {"custid", custId},
//                 {"fromdate", fromDate},
//                 {"todate", toDate},
//                 {"balanceupday", balanceUpDay},
//                 {"movebetday", moveBetDay},
//                 {"includeaccbal", includeAccBal},
//                 {"showexchangerate", false}
//             };
//
//             var result = new List<Statement>();
//
//             using (var reader = ExecDataReader("Acc_AllStatement_Full", parameters))
//             {
//                 if (reader != null)
//                 {
//                     while (reader.Read())
//                     {
//                         var type = reader.IsDBNull(reader.GetOrdinal("TransType"))
//                             ? string.Empty
//                             : reader["TransType"].ToString().ToUpper(CultureInfo.InvariantCulture);
//                         var remark = reader.IsDBNull(reader.GetOrdinal("TransDesc"))
//                             ? string.Empty
//                             : reader["TransDesc"].ToString();
//                         var isAccBal = string.Compare(remark.ToLower(CultureInfo.InvariantCulture), "accumulated balance",
//                                            StringComparison.OrdinalIgnoreCase) == 0;
//
//                         var entity = new Statement
//                         {
//                             AccDate = GetDate(type, isAccBal, Convert.ToDateTime(reader["AccDate"]), balanceUpDay),
//                             RefNo = reader.ToLong("RefNo"),
//                             TotalAmt = Convert.ToDouble(reader["TotalAmt"]),
//                             TransType = type,
//                             TransDesc = remark
//                         };
//                         result.Add(entity);
//                     }
//                 }
//             }
//
//             return result;
//         }
//
//         private static string GetDate(string transType, bool isAccBal, DateTime accDate, DateTime balanceUpDay)
//         {
//             if (string.Compare(transType.ToLower(CultureInfo.InvariantCulture), "bet", StringComparison.OrdinalIgnoreCase) == 0
//                 && accDate >= balanceUpDay)
//             {
//                 return accDate.DayOfWeek + "<br/>" + Utils.FormatDateOptionalTime(accDate);
//             }
//
//             return isAccBal ? "--" : accDate.DayOfWeek + " <br/>" + Utils.FormatDateOptionalTime(accDate);
//         }
//     }
// }