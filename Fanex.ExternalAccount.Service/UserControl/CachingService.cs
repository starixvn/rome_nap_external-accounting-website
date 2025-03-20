namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using Fanex.Caching;
    using Fanex.Caching.Memory;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    public interface ICachingService
    {
        UserCache GetUserCache(int userId, string groupName);
        void ClearUserCache();
        void RemoveUserCache(int userId);
    }
    
    public class CachingService : ICachingService
    {
        private readonly ICacheService UserCache = new CacheService(new MemoryCache(new MemoryCacheOptions()));
        private readonly IMenuDao _menuDao = new MenuDao();
        private string menuKey;

        public UserCache GetUserCache(int userId, string groupName)
        {
            menuKey = userId.ToString() + groupName;
            UserCache.TryGet<UserCache>(menuKey, out var result);

            if (result == null)
            {
                result = this._menuDao.GetMenuByUserId(userId);
                if (result != null)
                {
                    CheckPermissionForSearchAndCopy(result, groupName);
                    UserCache.Set<UserCache>(menuKey, result, new CacheItemOptions { RelativeExpiration = TimeSpan.FromSeconds(30) });
                }
            }

            return result;
        }

        public void RemoveUserCache(int userId)
        {
            UserCache.Remove(userId.ToString());
        }

        public void ClearUserCache()
        {
            if (!string.IsNullOrWhiteSpace(menuKey))
            {
                UserCache.Remove(menuKey);
            }
        }

        private static void CheckPermissionForSearchAndCopy(UserCache userCache, string groupName)
        {
            if(!AppSettings.ShowSearchAndCopyForGroups.Contains(groupName))
            {
                userCache.MenuList.RemoveAll(i => i.FuncKey == eFunctions.SearchAndCopySuper);
                userCache.PolicyList.RemoveAll(i => i.FuncKey == eFunctions.SearchAndCopySuper);
            }
        }
    }
}