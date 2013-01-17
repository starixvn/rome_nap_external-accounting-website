namespace Fanex.ExternalAccount.DataAccess.Interface.Login
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    /// <summary>
    /// ILoginDao
    /// </summary>
    public interface ILoginDao
    {
        Result Login(string username, string password, string browser, string ip, string domain, string countryCode, string platForm, out int custId);

        Result Logout(int custId, string username);

        Result Checkin(int custId, string netSessionID, string ip, out int onUserId);

        Result Recheckin(int custId, out bool isKickOut);

        ICollection<Group> GroupList();

        bool CheckExternalGroup(int custId, int funcId, string funcKey);
    }
}