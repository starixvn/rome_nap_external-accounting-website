namespace Fanex.ExternalAccount.DataAccess.Interface.Login
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ILoginDao
    {
        Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId);

        Result Logout(int custId, string username);

        Result Checkin(int custId, string netSessionId, string ip, out int onuserId);

        bool IsKickedOut(int custId);

        ICollection<Group> GroupList();

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);

        DateTime GetDbTime();
    }
}