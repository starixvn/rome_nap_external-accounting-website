// --------------------------------------------------------------------------
// <copyright file="LeftPanelService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.7.29">Created</change>
//   <change who="Andy.Nguyen" date="2013.9.26">View used credit and reset member credit</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class LeftPanelService : BaseService
    {
        private static int _totalPages = 1;
        private readonly ILeftPanelDao _data = new LeftPanelDao();

        public static int TotalPages
        {
            get { return LeftPanelService._totalPages; }
            set { LeftPanelService._totalPages = value; }
        }

        /// <summary>
        /// Gets all customer information on left panel.
        /// </summary>
        /// <param name="username">Username of customer.</param>
        /// <param name="groupName">Group name of customer.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <returns>CustomerModel object.</returns>
        public override CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1)
        {
            var ds = this._data.GetCustomerInfo(username, groupName);
            var model = new CustomerModel();

            if (ds != null && ds.Tables.Count > 0)
            {
                var tableCustomer = ds.Tables[0];
                var tableLine = ds.Tables[1];
                var tableCash = ds.Tables[2];
                var tableTotalMember = ds.Tables[3];
                var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);

                model.BalanceInfo(ds);
                model.PersonalInfo(ds);
                model.LoginInfo(ds);
                model.GetOnlineList(ds);
                model.GetDownlineList(ds, pageIndex);

                if (roleId == this.Super)
                {
                    model.SuperInfo(ds);
                }
                else if (roleId == this.Master)
                {
                    model.MasterInfo(ds);
                }
                else if (roleId == this.Agent)
                {
                    model.AgentInfo(ds);
                }
                else
                {
                    model.MemberInfo(ds);
                }

                return model;
            }

            return null;
        }

        /// <summary>
        /// Updates given credit.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>Result object.</returns>
        public override Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            return this._data.UpdateGivenCredit(id, uplineId, newGivenCredit);
        }

        /// <summary>
        /// Gets down-line customers.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <returns>DataTable object.</returns>
        public override DataTable GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            var dataTable = new DataTable();
            var dr = this._data.GetDownlineList(custid, pageSize, pageIndex);

            if (dr != null && dr.ResultSet != null && dr.ResultSet.Tables.Count > 0)
            {
                TotalPages = Utils.ToInt32(dr.Outputs["totalpages"]);

                return dr.ResultSet.Tables[0];
            }

            return null;
        }

        /// <summary>
        /// Gets online list customer.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <returns>DataRowCollection object.</returns>
        public override DataRowCollection GetOnlineList(int custid)
        {
            return this._data.GetOnlineList(custid);
        }

        public override IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId)
        {
            return this._data.MasterAgentUsedCreditItems(custId, roleId);
        }

        public override IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
        {
            return this._data.MemberUsedCreditItems(custId);
        }

        public override Result ResetMemberCredit(int custId, int agentId, bool closedOnly)
        {
            return this._data.ResetMemberCredit(custId, agentId, closedOnly);
        }
        
        public override Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
        {
            return this._data.CustomerInsertClearBalance(custId, custIds, isCheckedAll, ip);
        }

        public override DataTable CustomerSelectClearBalance(int custId)
        {
            return this._data.CustomerSelectClearBalance(custId);
        }
    }
}