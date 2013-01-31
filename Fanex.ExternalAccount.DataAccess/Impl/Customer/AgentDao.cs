// --------------------------------------------------------------------------
// <copyright file="AgentDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AgentDao : CustomerDao, IAgentDao
    {
        #region IData Agents

        /// <summary>
        /// Base function for Agent level
        /// </summary>
        /// <param name="recommend">Id of current level</param>
        /// <param name="custId">Id of selected user</param>
        /// <param name="enabled">Enable status</param>
        /// <param name="closed">Close status</param>
        /// <param name="suspended">Suspend status</param>
        /// <param name="groupName">Group name of currency</param>
        /// <returns>DataTable object</returns>
        public DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            var param = new Hashtable();
            param.Add("mrecommend", recommend);
            param.Add("custid", custId);
            param.Add("enabled", enabled);
            param.Add("closed", closed);
            param.Add("suspended", suspended);

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