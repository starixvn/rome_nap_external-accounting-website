namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class CustomerService : ICustomerService

    {
        private readonly ICustomerDao customerDao = new CustomerDao();

        public bool IsValidUsername(string userName, int uplineId, out int custId)
            => customerDao.IsValidUsername(userName, uplineId, out custId);

        public Customer UserInfo(int custId) => customerDao.UserInfo(custId);

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
            => customerDao.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, remark, ip,
                    userId, subAccId, language);

        public Result ResetPassword(int custId, string password, Request request)
            => customerDao.ResetPassword(custId, password, request);

        public Result ConfigDoubleComm(int custId, bool allow, Request request)
            => customerDao.ConfigDoubleComm(custId, allow, request);
    }
}