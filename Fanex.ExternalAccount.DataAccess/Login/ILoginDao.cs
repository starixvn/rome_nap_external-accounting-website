namespace Fanex.ExternalAccount.DataAccess.Interface.Login
{
    using Fanex.ExternalAccount.DTO;
    using System;

    public interface ILoginDao
    {
        Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId);

        void Logout(int custId, string username);

        int Checkin(int custId, string netSessionId, string ip);

        bool IsKickedOut(int custId);        

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDbTime();
    }
}