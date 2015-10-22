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
    using Fanex.Module.Logs.Services;

    /// <summary>
    /// Logs object.
    /// </summary>
    public static class Logs
    {
        public static void Error(System.Exception ex, bool includeQuery = true)
        {
            ex.Write();
        }
    }
}