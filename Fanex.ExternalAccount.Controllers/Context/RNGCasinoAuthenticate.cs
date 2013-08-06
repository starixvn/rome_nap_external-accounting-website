// -----------------------------------------------------------------------
// <copyright file="RNGCasinoAuthenticate.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Controllers.Context
{
    using System;
    using Fanex.CustomerSetting.Security.Authenticate;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class RNGCasinoAuthenticate : IAuthenticate
    {
        public bool AllowRead(string action)
        {
            return true;
        }

        public bool AllowUpdate(string action)
        {
            return true;
        }
    }
}