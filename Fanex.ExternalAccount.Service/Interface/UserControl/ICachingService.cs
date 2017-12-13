namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using System;
    using System.Text;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ICachingService
    {
        UserCache GetUserCache(int userId);
        void ClearUserCache();
        void RemoveUserCache(int userId);
    }
}