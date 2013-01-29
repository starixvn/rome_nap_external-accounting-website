// --------------------------------------------------------------------------
// <copyright file="AgentService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
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
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class AgentService : CustomerService, IAgentService
    {
        private readonly IAgentDao dao = new AgentDao();

        #region IDataService Agents

        public System.Data.DataTable GetData(int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            try
            {
                return this.dao.GetData(recommend, custId, enabled, closed, suspended);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Transfer GetTransferOption(int custId)
        {
            try
            {
                return this.dao.GetTransferOption(custId);
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