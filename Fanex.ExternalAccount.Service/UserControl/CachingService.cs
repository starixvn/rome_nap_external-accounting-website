namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using System.Linq;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    public class CachingService : ICachingService
    {
        private static readonly ICache UserCache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IMenuDao _menuDao = new MenuDao();

        public UserCache GetUserCache(int userId, string groupName)
        {
            var menuKey = userId.ToString() + groupName;
            var result = UserCache.Get<UserCache>(menuKey);

            if (result == null)
            {
                result = this._menuDao.GetMenuByUserId(userId);
                if (result != null)
                {
                    CheckPermissionForSearchAndCopy(result, groupName);
                    UserCache.Set<UserCache>(menuKey, result, DateTime.Now.AddSeconds(30));
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
            UserCache.Clear();
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