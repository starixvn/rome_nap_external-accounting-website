namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO.Customer;

    public class AgentDao : CustomerDao, IAgentDao
    {
        #region IData Agents

        public DataTable GetData(
            int? userId, 
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            var param = new Hashtable();
            param.Add("mrecommend", recommend);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);
            param.Add("userid", userId);

            return DbHelper.ExecuteDataTable("EAS_CustomerSelByMAg", param);
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
                    Wed = Convert.ToBoolean(int.Parse(transferOption[3].ToString())),
                    Thu = Convert.ToBoolean(int.Parse(transferOption[4].ToString())),
                    Fri = Convert.ToBoolean(int.Parse(transferOption[5].ToString())),
                    Sat = Convert.ToBoolean(int.Parse(transferOption[6].ToString())),
                    Sun = Convert.ToBoolean(int.Parse(transferOption[0].ToString()))
                };
            }

            return null;
        }

        #endregion IData Agents
    }
}