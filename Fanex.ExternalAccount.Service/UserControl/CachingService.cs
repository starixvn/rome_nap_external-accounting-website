namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    public class CachingService : ICachingService
    {
        private static readonly ICache UserCache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IMenuDao _menuDao = new MenuDao();

        public UserCache GetUserCache(int userId)
        {
            var result = UserCache.Get<UserCache>(userId.ToString());

            if (result == null)
            {
                result = this._menuDao.GetMenuByUserId(userId);
                if (result != null)
                {
#pragma warning disable S109 // Magic numbers should not be used
                    UserCache.Set<UserCache>(userId.ToString(), result, DateTime.Now.AddSeconds(30));
#pragma warning restore S109 // Magic numbers should not be used
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
    }
}