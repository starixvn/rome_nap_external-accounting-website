// --------------------------------------------------------------------------
// <copyright file="CacheManager.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.15">Created</change>
// </history>

using System;
using System.Diagnostics.CodeAnalysis;
using Fanex.Caching;

namespace Fanex.Puffin.Core.Web
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public static class CacheManager
    {
        private static readonly ICache _cacheManager;

        static CacheManager()
        {
            _cacheManager = CacheService.CreateInstance(CacheType.MemoryCache);
        }

        public static object Get(string key)
        {
            try
            {
                return _cacheManager.Get<object>(key);
            }
            catch (Exception)
            {
                return null;
            }
        }

        public static void Set(string key, object value)
        {
            try
            {
                _cacheManager.Set(key, value);
            }
            catch (Exception)
            {
            }
        }

        public static T Get<T>(string key)
        {
            try
            {
                return _cacheManager.Get<T>(key);
            }
            catch (Exception ex)
            {
                return default(T);
            }
        }

        public static void Set<T>(string key, T value)
        {
            _cacheManager.Set<T>(key, value);
        }

        public static void Set(string key, object value, TimeSpan timeSpanExpiration)
        {
            _cacheManager.Set(key, value, timeSpanExpiration);
        }

        public static void Set<T>(string key, T value, TimeSpan timeSpanExpiration)
        {
            _cacheManager.Set<T>(key, value, timeSpanExpiration);
        }

        public static void Set(string key, object value, DateTime absoluteExpiration)
        {
            _cacheManager.Set(key, value, absoluteExpiration);
        }

        public static void Set<T>(string key, T value, DateTime absoluteExpiration)
        {
            _cacheManager.Set<T>(key, value, absoluteExpiration);
        }

        public static void Remove(string key)
        {
            _cacheManager.Remove(key);
        }
    }
}