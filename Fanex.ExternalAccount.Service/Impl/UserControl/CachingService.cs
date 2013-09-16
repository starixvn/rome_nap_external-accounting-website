// --------------------------------------------------------------------------
// <copyright file="CachingService.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">Created</change>
// </history>

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
                    UserCache.Set<UserCache>(userId.ToString(), result, DateTime.Now.AddSeconds(30));
                }
            }

            return result;
        }

        internal void RemoveUserCache(int userId)
        {
            UserCache.Remove(userId.ToString());
        }

        internal void ClearUserCache()
        {
            UserCache.Clear();
        }
    }
}