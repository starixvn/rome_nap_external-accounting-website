// --------------------------------------------------------------------------
// <copyright file="Logs.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Diagnostics;
    using System.Reflection;
    using System.Web;
    using Fanex.Logging;

    /// <summary>
    /// Logs object.
    /// </summary>
    public static class Logs
    {
        public static void Write(string msg)
        {
            LogService.Error(msg);
        }

        /// <summary>
        /// Writes log.
        /// </summary>
        /// <param name="msg">Log message.</param>
        public static void Write(object msg)
        {
            if (msg is Exception)
            {
                LogService.Error(null, msg as Exception);
            }
            else if (msg != null)
            {
                LogService.Error(msg.ToString());
            }
        }

        #region ILogger Members

        public static void Critical(string message, string category)
        {
            LogService.Critical(message, category);
        }

        public static void Critical(string message)
        {
            LogService.Critical(message);
        }

        public static void Debug(string message, string category)
        {
            LogService.Debug(message, category);
        }

        public static void Debug(string message)
        {
            LogService.Debug(message);
        }

        public static void Error(string message, string category, System.Exception exception)
        {
            LogService.Error(message, category, exception);
        }

        public static void Error(string message, System.Exception exception)
        {
            LogService.Error(message, exception);
        }

        public static void Error(string message, string category)
        {
            LogService.Error(message, category);
        }

        public static void Error(string message)
        {
            LogService.Error(message);
        }

        public static void Info(string message, string category)
        {
            LogService.Info(message, category);
        }

        public static void Info(string message)
        {
            LogService.Info(message);
        }

        public static void Warn(string message, string category)
        {
            LogService.Warn(message, category);
        }

        public static void Warn(string message)
        {
            LogService.Warn(message);
        }

        #endregion ILogger Members
    }
}