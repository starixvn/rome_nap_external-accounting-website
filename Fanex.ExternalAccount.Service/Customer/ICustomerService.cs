namespace Fanex.ExternalAccount.Service.Interface.Customer
{
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

        Result ResetPassword(int custId, string password, Request request);

        Result ConfigDoubleComm(int custId, bool allow, Request request);
    }
}