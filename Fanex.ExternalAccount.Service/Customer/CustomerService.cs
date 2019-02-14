namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDao dataAccess;

        public CustomerService(ICustomerDao dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new CustomerDao();
        }

        public bool IsValidUsername(string userName, int uplineId)
            => dataAccess.IsValidUsername(userName, uplineId);

        public Customer UserInfo(int custId) => dataAccess.UserInfo(custId);

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
            => dataAccess.EditUserInfo(customer, ip, userId, subAccId, language);


        public Result ResetPassword(ResetPasswordCriteria criteria)
            => dataAccess.ResetPassword(criteria);

        public Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria)
            => dataAccess.ConfigDoubleComm(criteria);

        public Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria)
            => dataAccess.CheckNewCustomerIsReachLimitation(criteria);
    }
}