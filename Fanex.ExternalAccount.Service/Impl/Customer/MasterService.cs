// --------------------------------------------------------------------------
// <copyright file="MasterService.cs" company="">
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
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class MasterService : CustomerService, IMasterService
    {
        private readonly IMasterDao _dao = new MasterDao();

        #region IDataService Members

        /// <summary>
        /// Base function for all levels.
        /// </summary>
        /// <param name="recommend">Id of current level.</param>
        /// <param name="custId">Id of selected user.</param>
        /// <param name="enabled">Enable status.</param>
        /// <param name="closed">Close status.</param>
        /// <param name="suspended">Suspend status.</param>
        /// <returns>DataTable object.</returns>
        public System.Data.DataTable GetData(int? userId, int? recommend = null, int? custId = null, bool? enabled = null, bool? closed = null, bool? suspended = null)
        {
            try
            {
                return this._dao.GetData(userId, recommend, custId, enabled, closed, suspended);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Gets transfer options.
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

        /// <summary>
        /// Edits transfer options.
        /// </summary>
        /// <param name="transferOption">Transfer options.</param>
        /// <param name="custId">Customer id.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="language">Language to get data.</param>
        /// <returns>Result object.</returns>
        public Result EditTransferOption(string transferOption, int custId, int userId, string ip, int subAccId, string language)
        {
            try
            {
                return this._dao.EditTransferOption(transferOption, custId, userId, ip, subAccId, language);
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