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
    using Fanex.Module.Logs.Services;

    /// <summary>
    /// Logs object.
    /// </summary>
    public static class Logs
    {
        public static void Error(System.Exception ex, bool includeQuery = true)
        {
            LogService.Error(ex.Message, ex);
        }

        public static void CasinoError(ErrorException ex, bool includeQuery = true)
        {
            ex.WriteCasinoException();
        }
    }
}