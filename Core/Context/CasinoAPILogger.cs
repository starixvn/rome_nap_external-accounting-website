// -----------------------------------------------------------------------
// <copyright file="CasinoAPILogger.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Core.Context
{
    using System.Collections;
    using System.Web;
    using System.Xml;
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
            var custId = HttpContext.Current.Request["id"];
            var urlRef = HttpContext.Current.Request.UrlReferrer;
            custId = string.IsNullOrWhiteSpace(custId) ? HttpContext.Current.Request["custid"] : custId;
            if (string.IsNullOrWhiteSpace(custId) || custId == "0")
            {
                var qryCustId = urlRef.Query.Replace("?", string.Empty).Split('&');
                foreach (var item in qryCustId)
                {
                    if (item.ToLower().Contains("custid"))
                    {
                        custId = item.Split('=')[1];
                        break;
                    }
                }

                if (string.IsNullOrWhiteSpace(custId) || custId == "0")
                {
                    var tracking = new XmlDocument();
                    tracking.LoadXml(strackingValue);

                    try
                    {
                        var id = tracking.GetElementsByTagName("DocumentElement").Item(0).FirstChild;
                        var nodeId = id.SelectSingleNode("intIBCCustID");
                        nodeId = nodeId == null ? id.SelectSingleNode("IBCCustID") : nodeId;
                        custId = nodeId.FirstChild.Value;
                    }
                    catch
                    {
                    }
                }
            } 
            
            param.Add("systemid", 3);
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
