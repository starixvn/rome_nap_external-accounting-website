// --------------------------------------------------------------------------
// <copyright file="MasterDao.cs" company="">
// Copyright (c) . All rights reserved.
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
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MasterDao : CustomerDao, IMasterDao
    {
        #region IData Masters

        /// <summary>
        /// Base function for all levels.
        /// </summary>
        /// <param name="recommend">Id of current level.</param>
        /// <param name="custId">Id of selected user.</param>
        /// <param name="enabled">Enable status.</param>
        /// <param name="closed">Close status.</param>
        /// <param name="suspended">Suspend status.</param>
        /// <returns>DataTable object.</returns>
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

        /// <summary>
        /// Gets transfer options.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>Transfer object.</returns>
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

        /// <summary>
        /// Edits transfer options.
        /// </summary>
        /// <param name="transferOption">Transfer options.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="language">Language to get data.</param>
        /// <returns>Result object.</returns>
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