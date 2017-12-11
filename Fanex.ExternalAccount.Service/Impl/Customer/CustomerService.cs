namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using Fanex.ExternalAccount.DataAccess.Impl.Customer;
    using Fanex.ExternalAccount.DataAccess.Interface.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Logging.Extensions.Web;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDao _dao = new CustomerDao();

        #region ICustomerService Members

        public bool IsValidUsername(string userName, int uplineId, out int custId, out bool isSubAcc, out int roleId)
        {
            return this._dao.IsValidUsername(userName, uplineId, out custId, out isSubAcc, out roleId);
        }

        public Customer UserInfo(int custId)
        {
            try
            {
                return this._dao.UserInfo(custId);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Result EditUserInfo(
            int custId, 
            string firstName, 
            string lastName, 
            string homePhone, 
            string mobilePhone,
            string email,
            string fax,
            string remark, 
            string ip, 
            int userId, 
            int subAccId, 
            string language)
        {
            try
            {
                return this._dao.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, remark, ip, userId, subAccId, language);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public Result ResetPassword(int custId, string password, Request request)
        {
            try
            {
                return this._dao.ResetPassword(custId, password, request);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        public DTO.Result ConfigDoubleComm(int custId, bool allow, Request request)
        {
            try
            {
                return this._dao.ConfigDoubleComm(custId, allow, request);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }

            return null;
        }

        #endregion ICustomerService Members
    }
}