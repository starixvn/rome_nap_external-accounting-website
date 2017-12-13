namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class CustomerService : ICustomerService

    {
        private readonly ICustomerDao _dao = new CustomerDao();

        public bool IsValidUsername(string userName, int uplineId, out int custId, out bool isSubAcc, out int roleId)
            => _dao.IsValidUsername(userName, uplineId, out custId, out isSubAcc, out roleId);

        public Customer UserInfo(int custId) => _dao.UserInfo(custId);

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
            => _dao.EditUserInfo(custId, firstName, lastName, homePhone, mobilePhone, email, fax, remark, ip,
                    userId, subAccId, language);

        public Result ResetPassword(int custId, string password, Request request)
            => _dao.ResetPassword(custId, password, request);

        public Result ConfigDoubleComm(int custId, bool allow, Request request)
            => _dao.ConfigDoubleComm(custId, allow, request);
    }
}