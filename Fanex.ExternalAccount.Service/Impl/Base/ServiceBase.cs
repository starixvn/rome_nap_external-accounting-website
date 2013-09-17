// -----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="Nexcel Solution Co.Ltd">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Base
{
    using System;
    using System.Web;
    using Fanex.Logging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServiceBase
    {
        /// <summary>
        /// Processes exception.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <param name="category">Category to write log.</param>
        protected void ProcessException(Exception ex, string category = "ExternalAccount")
        {
            var message = ex.Message +
                             "<br>\r\nSOURCE: " + ex.Source +
                             "<br>\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                             "<br>\r\nREFERRER: " + HttpContext.Current.Request.UrlReferrer +
                             "<br>\r\nFORM: " + HttpContext.Current.Request.Form.ToString() +
                             "<br>\r\nQUERYSTRING: " + HttpContext.Current.Request.QueryString.ToString() +
                             "<br>\r\nTARGETSITE: " + ex.TargetSite +
                             "<br>\r\nSTACKTRACE: " + ex.StackTrace;

            LogService.Error(message, category);

            if (HttpContext.Current.IsDebuggingEnabled)
            {
                throw ex;
            }
        }
    }
}