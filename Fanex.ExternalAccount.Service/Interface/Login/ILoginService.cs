namespace Fanex.ExternalAccount.Service.Interface.Login
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ILoginService
    {
        Result Login(string username, string password, Request request, out int custId, out int onUserId, out bool isIntAcc);

        Result Logout(int custId, string username);

        Result Recheckin(int custId, out bool isKickOut);

        ICollection<Group> GroupList();

        bool CheckExternalGroup(int custId, int? funcId, string funcKey);
    }
}