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
    public class BaseService : ILeftPanelService
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
        public virtual CustomerModel GetCustomerInfo(string username, string groupName = "ACCDDD", int pageIndex = 1)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets down-lines of customer.
        /// </summary>
        /// <param name="custid">Id of customer.</param>
        /// <param name="pageSize">Page size.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>DataTable object.</returns>
        public virtual DataTable GetDownlineList(int custid, int pageSize, int pageIndex)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets online list user.
        /// </summary>
        /// <param name="custid">Customer id.</param>
        /// <returns>DataRowCollection object.</returns>
        public virtual DataRowCollection GetOnlineList(int custid)
        {
            throw new NotImplementedException();
        }        

        /// <summary>
        /// Updates given credit for customer.
        /// </summary>
        /// <param name="id">Id of customer.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>Result object.</returns>
        public virtual Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            throw new NotImplementedException();
        }

        public virtual IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId)
        {
            throw new NotImplementedException();
        }

        public virtual IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
        {
            throw new NotImplementedException();
        }

        public virtual Result ResetMemberCredit(int custId, int agentId, bool closedOnly)
        {
            throw new NotImplementedException();
        }

        public virtual Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
        {
            throw new NotImplementedException();
        }

        public virtual DataTable CustomerSelectClearBalance(int custId)
        {
            throw new NotImplementedException();
        }

        public virtual Result CloseCutomer(int custId, bool close, bool clearCredit, string custName)
        {
            throw new NotImplementedException();
        }

        public virtual Result SuspendCustomer(int custId, bool suspend)
        {
            throw new NotImplementedException();
        }

        public virtual Result AllowOutright(int custId, bool allow)
        {
            throw new NotImplementedException();
        }

        public virtual Result EnableCasino(int id, bool enable, bool enableDownline, bool synce, string custName)
        {
            throw new NotImplementedException();
        }

        public virtual Result EnableBingo(int id, bool enable, bool sync, bool enableDownline)
        {
            throw new NotImplementedException();
        }

        public virtual Result EnableNumberGame(int id, bool enable, bool enableDownline)
        {
            throw new NotImplementedException();
        }

        public virtual Result EnableLiveCasino(int id, bool enable, bool enableDownline)
        {
            throw new NotImplementedException();
        }

        public virtual Result EnableVirtualSports(int id, bool enable, bool enableDownline)
        {
            throw new NotImplementedException();
        }
        
        public virtual Result AutoMasterPT(int id, bool allow)
        {
            throw new NotImplementedException();
        }

        public virtual Result ResetSecurityCode(int id, string code, bool force)
        {
            throw new NotImplementedException();
        }
    }
}