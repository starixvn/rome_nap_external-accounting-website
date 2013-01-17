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
        private static readonly ICache userCache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IMenuDao menuDao = new MenuDao();

        public UserCache GetUserCache(int userId)
        {
            var result = userCache.Get<UserCache>(userId.ToString());

            if (result == null)
            {
                result = this.menuDao.GetMenuByUserId(userId);
                if (result != null)
                {
                    userCache.Set<UserCache>(userId.ToString(), result, DateTime.Now.AddSeconds(30));
                }
            }

            return result;
        }

        internal void RemoveUserCache(int userId)
        {
            userCache.Remove(userId.ToString());
        }

        internal void ClearUserCache()
        {
            userCache.Clear();
        }
    }
}