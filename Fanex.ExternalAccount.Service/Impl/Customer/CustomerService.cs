// -----------------------------------------------------------------------
// <copyright file="CustomerService.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

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
        private readonly ICustomerDao dao = new CustomerDao();

        #region ICustomerService Members

        public Customer UserInfo(int custId)
        {
            try
            {
                return this.dao.UserInfo(custId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Result EditUserInfo(int custId, string firstName, string lastName, string homePhone, string mobilePhone,
                                    string email, string fax, string ip, int userId, int subAccId, string language)
        {
            try
            {
                return this.dao.EditUserInfo(custId, firstName, lastName, homePhone,mobilePhone, email, fax, ip, userId, subAccId, language);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Transfer TransferOption(int custId)
        {
            try
            {
                return this.dao.TransferOption(custId);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Result EditTransferOption(int custId, Transfer transferOption, Request request)
        {
            try
            {
                return this.dao.EditTransferOption(custId, transferOption, request);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public Result ResetPassword(int custId, string password, Request request)
        {
            try
            {
                return this.dao.ResetPassword(custId, password, request);
            }
            catch (Exception ex)
            {
                this.ProcessException(ex);
            }

            return null;
        }

        public DTO.Result ConfigDoubleComm(int custId, bool allow, Request request)
        {
            try
            {
                return this.dao.ConfigDoubleComm(custId, allow, request);
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