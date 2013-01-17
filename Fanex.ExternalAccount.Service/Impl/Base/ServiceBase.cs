// -----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Base
{
    using System;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServiceBase
    {
        protected void ProcessException(Exception ex, string category = "ExternalAccount")
        {
            LogService.Error(ex.Message, category, ex);

            if (System.Web.HttpContext.Current.IsDebuggingEnabled)
            {
                throw ex;
            }
        }
    }
}