// --------------------------------------------------------------------------
// <copyright file="CustomerService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.16">Created</change>
// </history>

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Impl.Base;
    using Fanex.ExternalAccount.Service.Interface.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CustomerService : ServiceBase, ICustomerService
    {
        private readonly ICustomerDao _dao = new CustomerDao();

        #region ICustomerService Members

        public bool IsUsernameBelong(string userName, int uplineId, out int custId, out bool isSubAcc, out int roleId)
        {
            return this._dao.IsUsernameBelong(userName, uplineId, out custId, out isSubAcc, out roleId);
        }

        /// <summary>
        /// Gets user info.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <returns>Customer object.</returns>
        public Customer UserInfo(int custId)
        {
            try
            {
                return this._dao.UserInfo(custId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Edits user info.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="firstName">First name.</param>
        /// <param name="lastName">Last name.</param>
        /// <param name="homePhone">Home phone.</param>
        /// <param name="mobilePhone">Mobile phone.</param>
        /// <param name="email">Email address.</param>
        /// <param name="fax">Fax number.</param>
        /// <param name="ip">IP address.</param>
        /// <param name="userId">Admin user id.</param>
        /// <param name="subAccId">Sub-account id.</param>
        /// <param name="language">Language to get data.</param>
        /// <returns>Result object.</returns>
        public Result EditUserInfo(
            int custId, 
            string firstName, 
            string lastName, 
            string homePhone, 
            string mobilePhone,
            string email, 
            string fax, 
            string ip, 
            int userId, 
            int subAccId, 
            string language)
        {
            try
            {
                return this._dao.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, ip, userId, subAccId, language);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        /// <summary>
        /// Resets password.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="password">Password characters.</param>
        /// <param name="request">Request object.</param>
        /// <returns>Result object.</returns>
        public Result ResetPassword(int custId, string password, Request request)
        {
            try
            {
                return this._dao.ResetPassword(custId, password, request);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, "ExternalAccount", false);
            }

            return null;
        }

        /// <summary>
        /// Config double commission.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="allow">Allow double commission or not.</param>
        /// <param name="request">Request object.</param>
        /// <returns>Result object.</returns>
        public DTO.Result ConfigDoubleComm(int custId, bool allow, Request request)
        {
            try
            {
                return this._dao.ConfigDoubleComm(custId, allow, request);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        #endregion ICustomerService Members
    }
}