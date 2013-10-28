// --------------------------------------------------------------------------
// <copyright file="LeftPanelDao.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using Fanex.Data;
    using Fanex.Data.DNAide;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelDao : ILeftPanelDao
    {
        /// <summary>
        /// Gets customer info.
        /// </summary>
        /// <param name="username">Customer username.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>DataSet object.</returns>
        public DataSet GetCustomerInfo(string username, string groupName = "ACCDDD")
        {
            var customerInfo = new ReportService().GetCustomerInfo(groupName, 0, username, true);

            if (customerInfo != null && customerInfo.Rows.Count != 0)
            {
                var ds = new DataSet();
                var param = new Hashtable();
                param.Add("username", username);

                ds = DbHelper.ExecuteDataSet("Acc_CustomerSelAllInfo", param);

                return ds;
            }

            return null;
        }

        /// <summary>
        /// Update given credit.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>Result object.</returns>
        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdCredit"))
            {
                var param = new Hashtable();
                param.Add("userid", uplineId);
                param.Add("custid", id);
                param.Add("newcredit", newGivenCredit);
                param.Add("ip", CurrentContext.Instance.Ip4Address);
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Gets down-line list.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <returns>DBResult object.</returns>
        public DbResult GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            DbResult rs = null;
            var param = new Hashtable();

            param.Add("custid", custid);
            param.Add("pagesize", pageSize);
            param.Add("pageindex", pageIndex);
            param.Add("totalpages", 0); // 0: Paging

            rs = DbHelper.ExecuteDbResult("Acc_CustomerSelList", param);

            return rs;
        }

        public DataRowCollection GetOnlineList(int custid)
        {
            DbResult rs = null;
            var param = new Hashtable();
            param.Add("custid", custid);
            rs = DbHelper.ExecuteDbResult("Acc_CustomerSelOnline", param);

            return rs.ResultSet.Tables[0].Rows;
        }

        /// <summary>
        /// Gets master or agent used credit.
        /// </summary>
        /// <param name="custId">Customer id (master or agent).</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>List object.</returns>
        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId)
        {
            IList<MasterAgentUsedCreditItem> items = null;

            if (roleId == 4)
            {
                items = this.MasterUsedCreditItems(custId);
            }
            else if (roleId == 3)
            {
                items = this.AgentUsedCreditItems(custId);
            }

            return items;
        }

        /// <summary>
        /// Gets member used credit.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>List object.</returns>
        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
        {
            IList<MemberUsedCreditItem> items = new List<MemberUsedCreditItem>();

            using (ObjectDb db = new ObjectDb("Acc_CustomerSelUsedCredit_Ag"))
            {
                db.AddInParameter("agentid", DbType.Int32, custId);

                using (IDataReader reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MemberUsedCreditItem
                        {
                            Username = reader.ToString("username"),
                            FirstName = reader.ToString("firstname"),
                            LastName = reader.ToString("lastname"),
                            Amount = reader.ToDecimal("credit"),
                            IsClosed = reader.ToBoolean("closed")
                        });
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Resets member used credit.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="agentId">Upper customer id.</param>
        /// <param name="closedOnly">Indicates whether or not reset closed member only.</param>
        /// <returns>List object.</returns>
        public Result ResetMemberCredit(int custId, int agentId, bool closedOnly)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdResetMemberCredit", 600))
            {
                var param = new Hashtable();
                param.Add("userid", custId);
                param.Add("agentid", agentId);
                param.Add("closedonly", closedOnly);
                param.Add("ip", HttpContext.Current.Request.UserHostAddress);
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);

                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();

                var errorCode = Utils.ToInt32(objDb.GetParameterValue("errcode"));

                return new Result(errorCode, objDb.GetParameterValue("errmsg"));
            }
        }        

        /// <summary>
        /// Clears balance.
        /// </summary>
        /// <param name="custId">From customer id.</param>
        /// <param name="custIds">To customer ids.</param>
        /// <param name="isCheckedAll">Boolean status.</param>
        /// <param name="ip">IP address.</param>
        /// <returns>Result object.</returns>
        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
        {
            using (ObjectDb db = new ObjectDb("Acc_CustomerInsClearBalance"))
            {
                var param = new Hashtable();
                param.Add("fromcustid", custId);
                param.Add("tocustids", custIds);
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("cusstatus", !isCheckedAll ? string.Empty : "All");
                param.Add("ip", ip);

                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result
                {
                    ErrorCode = Utils.ToString(db.GetParameterValue("errcode")),
                    ErrorMessage = Utils.ToString(db.GetParameterValue("errmsg"))
                };
            }
        }

        public DataTable CustomerSelectClearBalance(int custId)
        {
            using (ObjectDb db = new ObjectDb("Acc_CustomerSelClearBalance"))
            {
                var param = new Hashtable();
                param.Add("custid", custId);
                var result = db.ExecuteDataSet(param);

                return result != null && result.Tables != null ? result.Tables[0] : null;
            }
        }

        /// <summary>
        /// Closes a customer.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="close">Close or not.</param>
        /// <param name="clearCredit">Clear credit or not.</param>
        /// <param name="custName">Customer username.</param>
        /// <returns>Result object.</returns>
        public Result CloseCutomer(int custId, bool close, bool clearCredit, string custName)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdClosed", 600))
            {
                Hashtable param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", custId);
                param.Add("closed", close);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("ip", CurrentContext.Instance.Ip4Address);
                objDb.AssignParamValues(param);

                objDb.ExecuteNonQuery();
                                
                if (Utils.ToInt32(objDb.GetParameterValue("errcode")) <= 0)
                {
                    return this.ClearCredit(CurrentSession.Instance.UserId, custName);
                }

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Suspends a customer.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="suspend">Suspended or not.</param>
        /// <returns>Result object.</returns>
        public Result SuspendCustomer(int custId, bool suspend)
        {
            using (ObjectDb objDB = new ObjectDb("Acc_CustomerUpdSuspended"))
            {
                var param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", custId);
                param.Add("suspended", suspend);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("ip", CurrentContext.Instance.Ip4Address);

                objDB.AssignParamValues(param);
                objDB.ExecuteNonQuery();

                return new Result(objDB.GetParameterValue("errcode"), objDB.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Allows outright for customer.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="allow">Allow or not.</param>
        /// <returns>Result object.</returns>
        public Result AllowOutright(int custId, bool allow)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdDisableOutright", 600))
            {
                var param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", custId);
                param.Add("disableoutright", !allow);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("ip", CurrentContext.Instance.Ip4Address);

                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }
                
        /// <summary>
        /// Enable casino status.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="enable">Enable or not.</param>
        /// <param name="enableDownline">Enable down-line or not.</param>
        /// <param name="sync">Synchronize or not.</param>
        /// <param name="custName">Customer username.</param>
        /// <returns>Result object.</returns>
        public Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName)
        {
            var result = CasinoManager.Instance.UpdateCSStatus(
                id, 
                sync, 
                !enable, 
                enableDownline,
                CurrentSession.Instance.UserId, 
                CurrentContext.Instance.Ip4Address, 
                0, 
                "En", 
                custName, 
                string.Empty);

            return new Result(result ? 0 : 1, result ? string.Empty : "Update casino status failed");
        }

        /// <summary>
        /// Enables bingo status.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="enable">Enable or not.</param>
        /// <param name="sync">Synchronized or not.</param>
        /// <param name="enableDownline">Enables down-line or not.</param>
        /// <returns>Result object.</returns>
        public Result EnableBingo(int id, bool enable, bool sync, bool enableDownline)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_UpdBingoStatus", 600))
            {
                Hashtable param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", id);
                param.Add("disabledbingo", !enable);
                param.Add("syncbg", sync);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("loginip", CurrentContext.Instance.Ip4Address);
                param.Add("enabledownline", enableDownline);

                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Enables number game.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="enable">Enables or not.</param>
        /// <param name="enableDownline">Enables down-line or not.</param>
        /// <returns>Result object.</returns>
        public Result EnableNumberGame(int id, bool enable, bool enableDownline)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdDisableNumber", 600))
            {
                var param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", id);
                param.Add("disablenumber", !enable);
                param.Add("language", "E");
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);
                param.Add("ip", CurrentContext.Instance.Ip4Address);
                param.Add("disabledownline", !enableDownline);

                objDb.AssignParamValues(param);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Enables Live casino.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="enable">Enables or not.</param>
        /// <param name="enableDownline">Enables down-line or not.</param>
        /// <returns>Result object.</returns>
        public Result EnableLiveCasino(int id, bool enable, bool enableDownline)
        {
            using (ObjectDb db = new ObjectDb("Acc_CustomerUpdDisableLiveCS", 300))
            {
                Hashtable param = new Hashtable();
                param.Add("userid", CurrentSession.Instance.UserId);
                param.Add("custid", id);
                param.Add("disabledlivecs", !enable);
                param.Add("language", "E");
                param.Add("ip", CurrentContext.Instance.Ip4Address);
                param.Add("enabledownlineslivecs", enableDownline);
                param.Add("errcode", 0);
                param.Add("errmsg", string.Empty);

                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result(db.GetParameterValue("errcode"), db.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Enables Virtual sports.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="enable">Enables or not.</param>
        /// <param name="enableDownline">Enables down-line or not.</param>
        /// <returns>Result object.</returns>
        public Result EnableVirtualSports(int id, bool enable, bool enableDownline)
        {
            Hashtable param = new Hashtable();
            param.Add("userid", CurrentSession.Instance.UserId);
            param.Add("custid", id);
            param.Add("disable", !enable);
            param.Add("language", "E");
            param.Add("ip", CurrentContext.Instance.Ip4Address);
            param.Add("disabledownlines", !enableDownline);
            param.Add("errcode", 0);
            param.Add("errmsg", string.Empty);

            using (ObjectDb db = new ObjectDb("Acc_CustomerUpdDisableVS", 120))
            {
                db.AssignParamValues(param);
                db.ExecuteNonQuery();

                return new Result(db.GetParameterValue("errcode"), db.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Allows master auto PT.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="allow">Allow or not.</param>
        /// <returns>Result object.</returns>
        public Result AutoMasterPT(int id, bool allow)
        {
            var param = new Hashtable();
            param.Add("userid", CurrentSession.Instance.UserId);
            param.Add("custid", id);
            param.Add("disableautompt", !allow);
            param.Add("language", "E");
            param.Add("errcode", 0);
            param.Add("errmsg", string.Empty);
            param.Add("ip", CurrentContext.Instance.Ip4Address);

            using (ObjectDb objDB = new ObjectDb("Acc_CustomerUpdMaster_DisableAutoMPT"))
            {
                objDB.AssignParamValues(param);
                objDB.ExecuteNonQuery();

                return new Result(objDB.GetParameterValue("errcode"), objDB.GetParameterValue("errmsg"));
            }
        }

        /// <summary>
        /// Resets security code.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="code">Security code.</param>
        /// <param name="force">Force change or not.</param>
        /// <returns>Result object.</returns>
        public Result ResetSecurityCode(int id, string code, bool force)
        {
            Hashtable outputParams = new Hashtable();
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdSecurityCode"))
            {
                objDb.AddInParameter("Custid", System.Data.DbType.Int32, id);
                objDb.AddInParameter("Userid", System.Data.DbType.Int32, CurrentSession.Instance.UserId);
                objDb.AddInParameter("IsForcedChangeSecurityCode", System.Data.DbType.Boolean, force);
                objDb.AddInParameter("Securitycode", System.Data.DbType.String, code);
                objDb.AddInParameter("Language", System.Data.DbType.String, "En");
                objDb.AddInParameter("IP", System.Data.DbType.String, CurrentContext.Instance.Ip4Address);
                objDb.AddOutParameter("ErrCode", System.Data.DbType.Int32, 8);
                objDb.AddOutParameter("ErrMsg", System.Data.DbType.String, 1000);
                objDb.ExecuteNonQuery();
                outputParams = objDb.GetOuputParams();

                return new Result(outputParams["ErrCode"], outputParams["ErrMsg"]);
            }
        }

        /// <summary>
        /// Gets master used credit.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>List object.</returns>
        private IList<MasterAgentUsedCreditItem> MasterUsedCreditItems(int custId)
        {
            IList<MasterAgentUsedCreditItem> items = new List<MasterAgentUsedCreditItem>();

            using (ObjectDb db = new ObjectDb("Acc_CustomerSelUsedCredit_SAg"))
            {
                db.AddInParameter("superid", DbType.Int32, custId);

                using (IDataReader reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MasterAgentUsedCreditItem
                        {
                            Username = reader.ToString("mastername"),
                            GivenCredit = reader.ToDecimal("givencredit"),
                            UsedCredit = reader.ToDecimal("usedcredit")
                        });
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Gets agent used credit.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>List object.</returns>
        private IList<MasterAgentUsedCreditItem> AgentUsedCreditItems(int custId)
        {
            IList<MasterAgentUsedCreditItem> items = new List<MasterAgentUsedCreditItem>();

            using (ObjectDb db = new ObjectDb("Acc_CustomerSelUsedCredit_MAg"))
            {
                db.AddInParameter("masterid", DbType.Int32, custId);

                using (IDataReader reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        items.Add(new MasterAgentUsedCreditItem
                        {
                            Username = reader.ToString("agentname"),
                            GivenCredit = reader.ToDecimal("givencredit"),
                            UsedCredit = reader.ToDecimal("usedcredit")
                        });
                    }
                }
            }

            return items;
        }

        /// <summary>
        /// Clears customer credit.
        /// </summary>
        /// <param name="userId">Customer id.</param>
        /// <param name="username">Customer username.</param>
        /// <returns>Result object.</returns>
        private Result ClearCredit(int userId, string username = null)
        {
            using (ObjectDb objDb = new ObjectDb("Acc_CustomerUpdClearCredit", 600))
            {
                var hash = new Hashtable();
                hash.Add("username", username);
                hash.Add("userid", userId);
                hash.Add("returnerrcode", 1);
                hash.Add("errcode", 0);
                hash.Add("errmsg", string.Empty);
                hash.Add("ip", CurrentContext.Instance.Ip4Address);
                objDb.AssignParamValues(hash);

                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }        
    }
}