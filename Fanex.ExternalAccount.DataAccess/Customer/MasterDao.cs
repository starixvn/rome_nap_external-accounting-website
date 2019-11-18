namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Data.DNAide;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class MasterDao : CustomerDao, IMasterDao
    {
        public DataTable GetData(
            int? userId,
            int? recommend = null,
            int? custId = null,
            bool? enabled = null,
            bool? closed = null,
            bool? suspended = null)
        {
            var param = new Hashtable
            {
                {"srecommend", recommend},
#pragma warning disable S1192 // String literals should not be duplicated
                {"custid", custId},
#pragma warning restore S1192 // String literals should not be duplicated
                {"enabled", enabled},
                {"closed", closed},
                {"suspended", suspended},
                {"userid", userId}
            };

            return DbHelper.ExecuteDataTable("EAS_CustomerSelBySAg", param);
        }

        public IEnumerable<int> GetSuperInAutomateTransfer(int custId)
        {
            var param = new Hashtable { { "custid", custId }};

            var result = DbHelper.ExecuteDbResult("Acc_CustomerSelSuperAutoTransfer", param);

            return result.ResultSet.Tables[0].Rows.Cast<DataRow>().Select(row => int.Parse(row["custId"].ToString()));
        }

        public Transfer GetTransferOption(int custId)
        {
            var param = new Hashtable { { "custid", custId }, { "transferoption", "00000000" } };

            var result = DbHelper.ExecuteDbResult("Acc_CustomerSelTransferOption", param);
            var transferOption = result.Outputs["transferoption"].ToString();
            if (transferOption.Length > 0)
            {
                return new Transfer
                {
#pragma warning disable S109 // Magic numbers should not be used
                    Weekly = Convert.ToBoolean(int.Parse(transferOption[7].ToString())),
                    Mon = Convert.ToBoolean(int.Parse(transferOption[1].ToString())),
                    Tue = Convert.ToBoolean(int.Parse(transferOption[2].ToString())),
                    Wed = Convert.ToBoolean(int.Parse(transferOption[3].ToString())),
                    Thu = Convert.ToBoolean(int.Parse(transferOption[4].ToString())),
                    Fri = Convert.ToBoolean(int.Parse(transferOption[5].ToString())),
                    Sat = Convert.ToBoolean(int.Parse(transferOption[6].ToString())),
                    Sun = Convert.ToBoolean(int.Parse(transferOption[0].ToString()))
#pragma warning restore S109 // Magic numbers should not be used
                };
            }

            return null;
        }

        public Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language)
        {
            var param = new Hashtable
            {
                {"custid", custId},
                {"transferoption", transferOption},
                {"userid", userId},
                {"ip", ip},
                {"subaccid", subAccId},
                {"language", language},
                {"returnerrcode", false},
                {"errcode", 0},
                {"errmsg", string.Empty}
            };

            var result = DbHelper.ExecuteDbResult("EAS_CustomerUpdTransferOption", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }
    }
}