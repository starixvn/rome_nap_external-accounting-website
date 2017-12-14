namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using DTO;
    using DTO.Customer;

    public interface ICustomerDao
    {
        bool IsValidUsername(string userName, int uplineId, out int custId);

        Customer UserInfo(int custId);

        Result EditUserInfo(
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
            string language);

        Result ResetPassword(int custId, string password, Request request);

        Result ConfigDoubleComm(int custId, bool allow, Request request);
    }
}