// -----------------------------------------------------------------------
// <copyright file="ServiceBase.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Service.Impl.Base
{
    using System;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ServiceBase
    {
        /// <summary>
        /// Processes exception.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <param name="includeQuery">To prevent getting secure information.</param>
        protected void ProcessException(Exception ex, bool includeQuery = true)
        {
            Logs.Error(ex, includeQuery);
        }
    }
}