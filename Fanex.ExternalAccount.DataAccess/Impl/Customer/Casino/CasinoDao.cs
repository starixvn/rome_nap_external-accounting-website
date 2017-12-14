namespace Fanex.ExternalAccount.DataAccess.Impl.Customer.Casino
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using CasinoLibrary.SBAgent;
    using Data.DNAide;
    using DTO.Customer.Casino;
    using Utilities;

    public class CasinoDao
    {
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan",
            Justification = "Reviewed. Suppression is OK here.")]
        public CasinoCustomer GetCustInfoIBC(int ibcCustId, int uplineId)
        {
            var param = new Hashtable {{"custid", ibcCustId}};
            var uplineParam = new Hashtable {{"custid", uplineId}};
            var dr = DbHelper.ExecuteDbResult("Age_CustomerSelUserInfo", param, 300);
            var upline = uplineId != 0
                ? DbHelper.ExecuteDbResult("Age_CustomerSelUserInfo", uplineParam, 300)
                : new DbResult(new Hashtable(), new DataSet());
            var customerDetail = new CasinoCustomer
            {
                UplineDisabledCasino = uplineId != 0 && (bool) upline.ResultSet.Tables[0].Rows[0]["disablecasino"]
            };

            if (dr.ResultSet.Tables[0].Rows.Count > 0)
            {
                var row = dr.ResultSet.Tables[0].Rows[0];
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

                if (upline?.ResultSet != null && upline.ResultSet.Tables.Count > 0
                    && upline.ResultSet.Tables[0].Rows.Count > 0)
                {
                    var drupline = upline.ResultSet.Tables[0].Rows[0];
                    customerDetail.UplineCasinoSync = Utils.ToBoolean(drupline["synccasino"]);
                }

                var customer = new Customer();

                try
                {
                    var dt = customer.GetDefaultMaxTransfer(customerDetail.UpperCustID, customerDetail.Currency,
                        uplineId == 0);

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
                catch (Exception exception)
                {
                    // TODO: Writelog
                }

                customerDetail.NoIBCData = false;
            }

            return customerDetail;
        }

        public bool UpdateCSStatus(
            int custid,
            bool syncCS,
            bool disabledCS,
            int userId,
            string ip,
            int subAccId,
            string lang)
        {
            var param = new Hashtable
            {
                {"custid", custid},
                {"disabledcs", disabledCS},
                {"synccs", syncCS},
                {"enabledownline", false},
                {"userid", userId},
                {"subaccid", subAccId},
                {"loginip", ip},
                {"language", lang}
            };

            var dr = DbHelper.ExecuteDbResult("Acc_UpdCSStatus", param, 300);

            return Utils.ToBoolean(dr.Outputs["status"]);
        }

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
            var param = new Hashtable
            {
                {"custid", custid},
                {"disabledcs", disabledCS},
                {"synccs", syncCS},
                {"enabledownline", isUpdDownline},
                {"userid", userId},
                {"subaccid", subAccId},
                {"loginip", ip},
                {"language", lang}
            };

            var dr = DbHelper.ExecuteDbResult("Acc_UpdCSStatus", param, 300);

            errMsg = dr.Outputs["errmsg"].ToString();
            return Utils.ToBoolean(dr.Outputs["status"]);
        }
    }
}