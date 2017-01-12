// --------------------------------------------------------------------------
// <copyright file="CasinoDao.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.4.11">Created</change>
// </history>

namespace Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino
{
    using System.Collections;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using CasinoLibrary.SBAgent;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.DTO.Customer.Casino;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoDao
    {
        /// <summary>
        /// Gets customer information.
        /// </summary>
        /// <param name="ibcCustId">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <returns>CasinoCustomer object.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public CasinoCustomer GetCustInfoIBC(int ibcCustId, int uplineId)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", ibcCustId);
            var uplineParam = new Hashtable();
            uplineParam.Add("custid", uplineId);
            DbResult dr = DbHelper.ExecuteDbResult("Age_CustomerSelUserInfo", param, 300);
            var upline = uplineId != 0 ? DbHelper.ExecuteDbResult("Age_CustomerSelUserInfo", uplineParam, 300) : new DbResult(new Hashtable(), new DataSet());
            DataRow row = null;
            CasinoCustomer customerDetail = new CasinoCustomer();
            customerDetail.UplineDisabledCasino = uplineId != 0 ? (bool)upline.ResultSet.Tables[0].Rows[0]["disablecasino"] : false;
            if (dr.ResultSet.Tables[0].Rows.Count > 0)
            {
                row = dr.ResultSet.Tables[0].Rows[0];
                customerDetail.UserName = Utils.ToString(row["username"]);
                customerDetail.UserName2 = Utils.ToString(row["username2"]);
                customerDetail.FirstName = Utils.ToString(row["firstname"]);
                customerDetail.LastName = Utils.ToString(row["lastname"]);
                customerDetail.Phone = Utils.ToString(row["phone"]);
                customerDetail.MobilePhone = Utils.ToString(row["mobilephone"]);
                customerDetail.Fax = Utils.ToString(row["fax"]);
                customerDetail.Email = Utils.ToString(row["email"]);
                customerDetail.RoleId = Utils.ToInt32(row["roleid"]);
                customerDetail.Currency = Utils.ToInt32(row["currency"]);
                customerDetail.Closed = Utils.ToBoolean(row["closed"]);
                customerDetail.Suspend = Utils.ToBoolean(row["suspend"]);
                customerDetail.SIBCCustID = Utils.ToInt32(row["superid"]);
                customerDetail.MIBCCustID = Utils.ToInt32(row["masterid"]);
                customerDetail.AIBCCustID = Utils.ToInt32(row["agentid"]);
                customerDetail.GivenCredit = Utils.ToDecimal(row["credit"]).Format();
                customerDetail.CasinoSync = Utils.ToBoolean(row["synccasino"]);
                customerDetail.CasinoDisabled = Utils.ToBoolean(row["disablecasino"]);
                customerDetail.LiveCasinoSync = Utils.ToBoolean(row["synclivecasino"]);
                customerDetail.CasinoDisabled = Utils.ToBoolean(row["disablecasino"]);
                customerDetail.IBCCustID = Utils.ToInt32(row["custid"]);
                customerDetail.UserPwd = Utils.ToString(row["userpwd"]);
                customerDetail.UpperCustID = uplineId;
                customerDetail.IsEnablePT = true;

                if (upline != null && upline.ResultSet != null && upline.ResultSet.Tables.Count > 0 && upline.ResultSet.Tables[0].Rows.Count > 0)
                {
                    var drupline = upline.ResultSet.Tables[0].Rows[0];
                    customerDetail.UplineCasinoSync = Utils.ToBoolean(drupline["synccasino"]);
                }

                // Get max transfer information
                CasinoLibrary.SBAgent.Customer customer = new CasinoLibrary.SBAgent.Customer();
                DataTable dt = null;

                try
                {
                    dt = customer.GetDefaultMaxTransfer(customerDetail.UpperCustID, customerDetail.Currency, uplineId == 0);

                    if (customerDetail.RoleId != 4 && dt != null && dt.Rows.Count > 0)
                    {
                        customerDetail.MaxTransferCreditLimit = Utils.ToDouble(dt.Rows[0]["MaxLimit"]);
                        customerDetail.MinTransferCreditLimit = Utils.ToDouble(dt.Rows[0]["MinLimit"]);
                        customerDetail.DefaultTransferCredit = Utils.ToDouble(dt.Rows[0]["DefMaxTransferCredit"]);
                    }
                    else
                    {
                        customerDetail.MaxTransferCreditLimit = Utils.ToDouble(dt.Rows[0]["MaxTransferCredit"]);
                        customerDetail.MinTransferCreditLimit = customerDetail.MaxTransferCreditLimit;
                        customerDetail.DefaultTransferCredit = Utils.ToDouble(dt.Rows[0]["SuperMaxTransfer"]);
                    }
                }
                catch 
                {
                }

                customerDetail.NoIBCData = false;
            }

            return customerDetail;
        }

        /// <summary>
        /// Gets casino status.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <returns>Boolean object.</returns>
        public bool GetCasinoStatus(int custid)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);

            DbResult dr = Fanex.Data.DNAide.DbHelper.ExecuteDbResult("Age_GetCSStatus", param, 300);
            return Utils.ToBoolean(dr.Outputs["status"]);
        }

        /// <summary>
        /// Updates casino status.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="syncCS">Synchronize casino status.</param>
        /// <param name="disabledCS">Disabled casino status.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="ip">IP information.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="lang">Language to get information.</param>
        /// <param name="username">Username of customer.</param>
        /// <param name="subUsername">Sub-username of customer.</param>
        /// <returns>Boolean object.</returns>
        public bool UpdateCSStatus(
            int custid, 
            bool syncCS, 
            bool disabledCS, 
            int userId, 
            string ip, 
            int subAccId, 
            string lang, 
            string username, 
            string subUsername)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", disabledCS);
            param.Add("synccs", syncCS);
            param.Add("enabledownline", false);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult dr = Fanex.Data.DNAide.DbHelper.ExecuteDbResult("Acc_UpdCSStatus", param, 300);

            bool status = Utils.ToBoolean(dr.Outputs["status"]);

            if (!status)
            {
                return false;
            }

            Customer cust = new Customer();
            return cust.UpdateCustEnableCasino(custid, !disabledCS, true, ip, custid, subAccId == 0 ? username : subUsername, 2);
        }

        /// <summary>
        /// Updates casino status.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="syncCS">Synchronize casino status.</param>
        /// <param name="disabledCS">Disabled casino status.</param>
        /// <param name="isUpdDownline">Update down-line status.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="ip">IP information.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="lang">Language to get information.</param>
        /// <param name="errMsg">Error message.</param>        
        /// <returns>Boolean object.</returns>
        public bool DoUpdateCSStatusWithoutSync(
            int custid, 
            bool syncCS, 
            bool disabledCS, 
            bool isUpdDownline, 
            int userId, 
            string ip, 
            int subAccId, 
            string lang, 
            out string errMsg)
        {
            Hashtable param = new Hashtable();
            param.Add("custid", custid);
            param.Add("disabledcs", disabledCS);
            param.Add("synccs", syncCS);
            param.Add("enabledownline", isUpdDownline);

            param.Add("userid", userId);
            param.Add("subaccid", subAccId);
            param.Add("loginip", ip);
            param.Add("language", lang);

            DbResult dr = Fanex.Data.DNAide.DbHelper.ExecuteDbResult("Acc_UpdCSStatus", param, 300);

            errMsg = dr.Outputs["errmsg"].ToString();
            return Utils.ToBoolean(dr.Outputs["status"]);
        }
    }
}
