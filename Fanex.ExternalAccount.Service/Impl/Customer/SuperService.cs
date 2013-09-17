// --------------------------------------------------------------------------
// <copyright file="SuperService.cs" company="Nexcel Solutions Vietnam">
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

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SuperService : CustomerService, ISuperService
    {
        private readonly ISuperDao _dao = new SuperDao();

        #region IDataService Members

        /// <summary>
        /// Gets super list.
        /// </summary>
        /// <param name="recommend">Super agent id.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="enabled">Enabled status.</param>
        /// <param name="closed">Closed status.</param>
        /// <param name="suspended">Suspended status.</param>
        /// <param name="groupName">Group name.</param>
        /// <returns>DataTable object.</returns>
        public System.Data.DataTable GetData(
            int? recommend = null, 
            int? custId = null, 
            bool? enabled = null, 
            bool? closed = null, 
            bool? suspended = null, 
            string groupName = "ACCDDD")
        {
            try
            {
                return this._dao.GetData(recommend, custId, enabled, closed, suspended, groupName);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion IDataService Members
    }
}