namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    public interface ICustomerService
    {
        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId);

        Result EditUserInfo(Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCriteria criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria);
    }
}