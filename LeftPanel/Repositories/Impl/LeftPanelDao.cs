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
    }
}