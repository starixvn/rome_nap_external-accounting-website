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
    using System.Data.SqlClient;
    using System.Web;
    using Fanex.Logging;

    /// <summary>
    /// Logs object.
    /// </summary>
    public static class Logs
    {
        public static void Error(System.Exception ex, bool includeQuery = true)
        {
            if (Config.IsUM)
            {
                return;
            }

            var message = ex.Message +
                            "<br>\r\nSOURCE: " + ex.Source +
                            "<br>\r\nDOMAIN: " + HttpContext.Current.Request.Headers["Host"] +
                            "<br>\r\nREFERRER: " + (includeQuery ? Utils.ToString(HttpContext.Current.Request.UrlReferrer) : string.Empty) +
                            "<br>\r\nFORM: " + (includeQuery ? HttpContext.Current.Request.Form.ToString() : string.Empty) +
                            "<br>\r\nQUERYSTRING: " + (includeQuery ? HttpContext.Current.Request.QueryString.ToString() : string.Empty) +
                            "<br>\r\nTARGETSITE: " + ex.TargetSite +
                            "<br>\r\nSTACKTRACE: " + ex.StackTrace +
                            ex.FormatSqlException();

            LogService.Error(message);
        }

        private static string FormatSqlException(this System.Exception ex)
        {
            var result = string.Empty;
            var sqlEx = ex as SqlException;

            if (sqlEx != null)
            {
                result = "<br>\r\nProcedure: " + sqlEx.Procedure +
                    "<br>\r\nLine Number: " + sqlEx.LineNumber +
                    "<br>\r\nServer: " + sqlEx.Server;
            }

            return result;
        }
    }
}