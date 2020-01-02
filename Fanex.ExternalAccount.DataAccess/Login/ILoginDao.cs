namespace Fanex.ExternalAccount.DataAccess.Interface.Login
{
    using Fanex.ExternalAccount.DTO;
    using System;

    public interface ILoginDao
    {
        Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId);

        Result Logout(int custId, string username);

        Result Checkin(int custId, string netSessionId, string ip, out int onuserId);

        bool IsKickedOut(int custId);        

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDbTime();
    }
}