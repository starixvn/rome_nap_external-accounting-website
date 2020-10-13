namespace Fanex.ExternalAccount.Service.Interface.UserControl
{
    using Fanex.ExternalAccount.DTO.UserControl;

    public interface ICachingService
    {
        UserCache GetUserCache(int userId, string groupName);
        void ClearUserCache();
        void RemoveUserCache(int userId);
    }
}