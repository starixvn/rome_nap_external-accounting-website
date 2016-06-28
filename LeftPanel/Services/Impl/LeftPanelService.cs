// --------------------------------------------------------------------------
// <copyright file="LeftPanelService.cs" company="">
// Copyright (c) . All rights reserved.
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
    using CasinoLibrary.SBAgent;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Repositories.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
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

                model.HasDownline = Utils.ToBoolean(tableCustomer.Rows[0]["HasDownline"]);
                model.BalanceInfo(ds);
                model.PersonalInfo(ds);
                model.LoginInfo(ds);

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

        public override Result CloseCutomer(int custId, bool close, bool clearCredit, string custName)
        {
            return this._data.CloseCutomer(custId, close, clearCredit, custName);
        }

        public override Result SuspendCustomer(int custId, bool suspend)
        {
            return this._data.SuspendCustomer(custId, suspend);
        }

        public override Result AllowOutright(int custId, bool allow)
        {
            return this._data.AllowOutright(custId, allow);
        }

        public override Result EnableBingo(int id, bool enable, bool sync, bool enableDownline)
        {
            return this._data.EnableBingo(id, enable, sync, enableDownline);
        }

        public override Result EnableCasino(int id, bool enable, bool enableDownline, bool synce, string custName)
        {
            return this._data.EnableCasino(id, enable, enableDownline, synce, custName);
        }

        public override Result EnableLiveCasino(int id, bool enable, bool enableDownline)
        {
            return this._data.EnableLiveCasino(id, enable, enableDownline);
        }

        public override Result EnableNumberGame(int id, bool enable, bool enableDownline)
        {
            return this._data.EnableNumberGame(id, enable, enableDownline);
        }

        public override Result EnableVirtualSports(int id, bool enable, bool enableDownline)
        {
            return this._data.EnableVirtualSports(id, enable, enableDownline);
        }

        public override Result EnableESports(int id, bool enable, bool enableDownline)
        {
            return this._data.EnableESports(id, enable, enableDownline);
        }

        /// <summary>
        /// Auto update master and sync to casino database.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="allow">Allow PT or not.</param>
        /// <param name="uplineId">Up line id.</param>
        /// <returns>Result of actions.</returns>
        public override Result AutoMasterPT(int id, bool allow, int uplineId)
        {
            var result = this._data.AutoMasterPT(id, allow);

            CasinoManager casinoManager = new CasinoManager();
            var custInfo = casinoManager.GetCustInfoIBC(id, uplineId);

            if (custInfo.CasinoSync)
            {
                var casinoSync = new Customer();
                casinoSync.UpdateMasterDisableAutoMPT(
                    id,
                    allow,
                    CurrentContext.Instance.Ip4Address,
                    CurrentSession.Instance.UserId,
                    CurrentSession.Instance.UserName,
                    3);
            }

            return result;
        }

        public override Result ResetSecurityCode(int id, string code, bool force)
        {
            return this._data.ResetSecurityCode(id, code, force);
        }

        public override Result EnableHRFixedOdds(int id, bool enable, bool enableDownline)
        {
            return this._data.EnableHRFixedOdds(id, enable, enableDownline);
        }

        public override Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId)
        {
            return this._data.UpdateGoldDeluxe(id, enable, enableDownline, custName, userId);
        }
    }
}