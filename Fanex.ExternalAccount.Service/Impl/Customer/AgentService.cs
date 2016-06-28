// --------------------------------------------------------------------------
// <copyright file="AgentService.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.28">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AgentService : CustomerService, IAgentService
    {
        private readonly IAgentDao _dao = new AgentDao();

        #region IDataService Agents

        /// <summary>
        /// Gets data for customer.
        /// </summary>
        /// <param name="recommend">Agent id of customer.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <returns>DataTable object.</returns>
        public System.Data.DataTable GetData(
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null)
        {
            try
            {
                return this._dao.GetData(recommend, custId, enabled, closed, suspended);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets transfer option.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>Transfer object.</returns>
        public Transfer GetTransferOption(int custId)
        {
            try
            {
                return this._dao.GetTransferOption(custId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion IDataService Agents
    }
}