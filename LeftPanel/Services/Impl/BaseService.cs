// --------------------------------------------------------------------------
// <copyright file="BaseService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Impl
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public abstract class BaseService : ILeftPanelService
    {
        public int Agent 
        { 
            get 
            { 
                return 2; 
            } 
        }

        public int Master 
        { 
            get 
            { 
                return 3; 
            } 
        }

        public int Member 
        { 
            get 
            { 
                return 1; 
            }
        }

        public int Super
        {
            get
            {
                return 4;
            }
        }        

        /// <summary>
        /// Gets customer information.
        /// </summary>
        /// <param name="username">Username of customer.</param>
        /// <param name="groupName">Group name of customer.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>CustomerModel object.</returns>
        public abstract CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1);

        /// <summary>
        /// Gets down-lines of customer.
        /// </summary>
        /// <param name="custid">Id of customer.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>DataTable object.</returns>
        public abstract DataTable GetDownlineList(int custid, int pageSize, int pageIndex);

        /// <summary>
        /// Gets online list user.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <returns>DataRowCollection object.</returns>
        public abstract DataRowCollection GetOnlineList(int custid);

        /// <summary>
        /// Updates given credit for customer.
        /// </summary>
        /// <param name="id">Id of customer.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>Result object.</returns>
        public abstract Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        public abstract IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId);

        public abstract IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId);

        public abstract Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        public abstract Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        public abstract DataTable CustomerSelectClearBalance(int custId);

        public abstract Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        public abstract Result SuspendCustomer(int custId, bool suspend);

        public abstract Result AllowOutright(int custId, bool allow);

        public abstract Result EnableCasino(int id, bool enable, bool enableDownline, bool synce, string custName);

        public abstract Result EnableBingo(int id, bool enable, bool sync, bool enableDownline);

        public abstract Result EnableNumberGame(int id, bool enable, bool enableDownline);

        public abstract Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        public abstract Result EnableVirtualSports(int id, bool enable, bool enableDownline);

        public abstract Result EnableESports(int id, bool enable, bool enableDownline);

        public abstract Result AutoMasterPT(int id, bool allow, int uplineId);

        public abstract Result ResetSecurityCode(int id, string code, bool force);

        public abstract Result EnableHRFixedOdds(int id, bool enable, bool enableDownline);
    }
}