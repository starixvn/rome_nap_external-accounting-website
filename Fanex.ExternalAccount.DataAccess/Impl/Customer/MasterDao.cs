namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    public class MasterDao : CustomerDao, IMasterDao
    {
        #region IData Masters

        public DataTable GetData(
            int? userId, 
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            var param = new Hashtable();
            param.Add("srecommend", recommend);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);
            param.Add("userid", userId);

            return DbHelper.ExecuteDataTable("EAS_CustomerSelBySAg", param);
        }

        public Transfer GetTransferOption(int custId)
        {
            var param = new Hashtable();
            param.Add("custid", custId);
            param.Add("transferoption", "00000000");

            var result = DbHelper.ExecuteDbResult("Acc_CustomerSelTransferOption", param);
            var transferOption = result.Outputs["transferoption"].ToString();
            if (transferOption.Length > 0)
            {
                return new Transfer()
                {
                    Weekly = Convert.ToBoolean(int.Parse(transferOption[7].ToString())),
                    Mon = Convert.ToBoolean(int.Parse(transferOption[1].ToString())),
                    Tue = Convert.ToBoolean(int.Parse(transferOption[2].ToString())),
                    Wed = Convert.ToBoolean(int.Parse(transferOption[3].ToString())),
                    Thu = Convert.ToBoolean(int.Parse(transferOption[4].ToString())),
                    Fri = Convert.ToBoolean(int.Parse(transferOption[5].ToString())),
                    Sat = Convert.ToBoolean(int.Parse(transferOption[6].ToString())),
                    Sun = Convert.ToBoolean(int.Parse(transferOption[0].ToString()))
                };
            }

            return null;
        }

        public Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language)
        {
            var param = new Hashtable();

            param.Add("custid", custId);
            param.Add("transferoption", transferOption);
            param.Add("userid", userId);
            param.Add("ip", ip);
            param.Add("subaccid", subAccId);
            param.Add("language", language);
            param.Add("returnerrcode", false);
            param.Add("errcode", 0);
            param.Add("errmsg", string.Empty);

            var result = DbHelper.ExecuteDbResult("EAS_CustomerUpdTransferOption", param);

            return new Result(result.Outputs["errcode"].ToString(), result.Outputs["errmsg"].ToString(), result.ResultSet);
        }

        #endregion IData Masters
    }
}