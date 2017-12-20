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

        public bool IsValidUsername(string userName, int uplineId)
            => _dao.IsValidUsername(userName, uplineId);

        public Customer UserInfo(int custId) => _dao.UserInfo(custId);

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
            => _dao.EditUserInfo(customer, ip, userId, subAccId, language);


        public Result ResetPassword(int custId, string password, Request request)
            => _dao.ResetPassword(custId, password, request);

        public Result ConfigDoubleComm(int custId, bool allow, Request request)
            => _dao.ConfigDoubleComm(custId, allow, request);
    }
}