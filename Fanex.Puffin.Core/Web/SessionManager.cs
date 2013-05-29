// --------------------------------------------------------------------------
// <copyright file="SessionManager.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.22">Created</change>
// </history>

using System;
using System.Web;

namespace Fanex.Puffin.Core.Web
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SessionManager
    {
        /// <summary>
        /// Set Value to session with specific type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionKey"></param>
        /// <param name="value"></param>
        public static void Set<T>(string sessionKey, T value)
        {
            try
            {
                HttpContext.Current.Session[sessionKey] = value;
            }
            catch (Exception)
            {
            }
        }

        /// <summary>
        /// Get Value from Session with specific Type
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static T Get<T>(string sessionKey)
        {
            try
            {
                T value = (T)HttpContext.Current.Session[sessionKey];
                return value;
            }
            catch (Exception)
            {
                return default(T);
            }
        }

        /// <summary>
        /// Set object to Session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <param name="value"></param>
        public static void Set(string sessionKey, object value)
        {
            SessionManager.Set<object>(sessionKey, value);
        }

        /// <summary>
        /// Get value object from Session
        /// </summary>
        /// <param name="sessionKey"></param>
        /// <returns></returns>
        public static object Get(string sessionKey)
        {
            return SessionManager.Get<object>(sessionKey);
        }
    }
}