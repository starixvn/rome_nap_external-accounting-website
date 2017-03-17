// --------------------------------------------------------------------------
// <copyright file="Logs.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Utilities
{
    using CasinoLibrary.Model.UserException;
    using Fanex.Logging;

    /// <summary>
    /// Logs object.
    /// </summary>
    public static class Logs
    {
        public static void Error(System.Exception ex, bool includeQuery = true)
        {
            LogService.Error(ex.Message);
        }

        public static void CasinoException(ErrorException ex, bool includeQuery = true)
        {
            LogService.Error(ex.Message, ex);
        }
    }
}