// -----------------------------------------------------------------------
// <copyright file="ICustomerDao.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public interface ICustomerDao
    {
        Customer UserInfo(int custId);

        Result EditUserInfo(int custId, string firstName, string lastName, string homePhone, string mobilePhone,
                                    string email, string fax, string ip, int userId, int subAccId, string language);



        Result ResetPassword(int custId, string password, Request request);

        Result ConfigDoubleComm(int custId, bool allow, Request request);
    }
}