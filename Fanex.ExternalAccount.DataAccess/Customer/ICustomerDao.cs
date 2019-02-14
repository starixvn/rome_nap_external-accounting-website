namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using DTO;
    using DTO.Customer;

    public interface ICustomerDao
    {
        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId);

        Result EditUserInfo(
            Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCriteria criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria);

        Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria);
    }
}