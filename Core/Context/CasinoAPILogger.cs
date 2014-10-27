// -----------------------------------------------------------------------
// <copyright file="CasinoAPILogger.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Core.Context
{
    using System.Collections;
    using System.Web;
    using Fanex.Data;
    using Fanex.ExternalAccount.Utilities;
    using Sunplus.Libraries.APILogger;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class CasinoAPILogger : IAPILogger
    {
        /// <summary>
        /// Write log.
        /// </summary>
        /// <param name="actionId">Action ID.</param>
        /// <param name="functionName">Function Name.</param>
        /// <param name="strackingValue">Tracking Value.</param>
        /// <param name="returnValue">Return Value.</param>
        /// <param name="returnCode">Error Code.</param>
        public void Write(int actionId, string functionName, string strackingValue, string returnValue, int returnCode)
        {
            var param = new Hashtable();
            var custId = string.IsNullOrWhiteSpace(HttpContext.Current.Request["id"]) ? HttpContext.Current.Request["custid"] : HttpContext.Current.Request["id"];
            param.Add("systemid", 4);
            param.Add("userid", CurrentSession.Instance.UserId);
            param.Add("custid", Utils.ToInt32(custId));
            param.Add("actionid", actionId);
            param.Add("funcname", functionName);
            param.Add("trackingvalue", strackingValue);
            param.Add("returnvalue", returnValue);
            param.Add("returncode", returnCode);

            using (ObjectDb db = new ObjectDb("WriteLog_UserLog_CasinoAPI", 600))
            {
                db.AssignParamValues(param);
                var ds = db.ExecuteNonQuery();
            }
        }
    }
}
