namespace Fanex.ExternalAccount.Service.Impl.UserControl
{
    using System;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DataAccess.Impl.UserControl;
    using Fanex.ExternalAccount.DataAccess.Interface.UserControl;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Interface.UserControl;

    /// <summary>
    /// Caching service object.
    /// </summary>
    public class CachingService : ICachingService
    {
        private static readonly ICache UserCache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IMenuDao _menuDao = new MenuDao();

        /// <summary>
        /// Gets user cache.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <returns>UserCache object.</returns>
        public UserCache GetUserCache(int userId)
        {
            var result = UserCache.Get<UserCache>(userId.ToString());

            if (result == null)
            {
                result = this._menuDao.GetMenuByUserId(userId);
                if (result != null)
                {
                    UserCache.Set<UserCache>(userId.ToString(), result, DateTime.Now.AddSeconds(30));
                }
            }

            return result;
        }

        /// <summary>
        /// Removes user cache.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        public void RemoveUserCache(int userId)
        {
            UserCache.Remove(userId.ToString());
        }

        /// <summary>
        /// Clears user cache.
        /// </summary>
        internal void ClearUserCache()
        {
            UserCache.Clear();
        }
    }
}