namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections;
    using System.Globalization;
    using System.Web;
    using System.Xml;
    using Data;
    using Sunplus.Libraries.APILogger;
    using Utilities;

    public class CasinoAPILogger : IAPILogger
    {
        public void Write(int actionId, string functionName, string strackingValue, string returnValue, int returnCode)
        {
            var custId = HttpContext.Current.Request["id"];
            var urlRef = HttpContext.Current.Request.UrlReferrer;

            custId = string.IsNullOrWhiteSpace(custId) ? HttpContext.Current.Request["custid"] : custId;
            if (urlRef != null)
            {
                var qryCustId = urlRef.Query.Replace("?", string.Empty).Split('&');
                if (string.IsNullOrWhiteSpace(custId) || custId == "0")
                {
                    foreach (var item in qryCustId)
                    {
                        if (item.ToLower(CultureInfo.InvariantCulture).Contains("custid"))
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
                            var id = tracking.GetElementsByTagName("DocumentElement").Item(0)?.FirstChild;
                            var nodeId = id?.SelectSingleNode("intIBCCustID");
                            if (id != null)
                            {
                                nodeId = nodeId ?? id.SelectSingleNode("IBCCustID");
                            }

                            if (nodeId != null)
                            {
                                custId = nodeId.FirstChild.Value;
                            }
                        }
                        catch
                        {
                            // TODO: Writelog
                        }
                    }
                }
            }
            var param = new Hashtable
            {
                {"systemid", 3},
                {"userid", CurrentSession.Instance.UserId},
                {"custid", Utils.ToInt32(custId)},
                {"actionid", actionId},
                {"funcname", functionName},
                {"trackingvalue", strackingValue},
                {"returnvalue", returnValue},
                {"returncode", returnCode}
            };

            using (var db = new ObjectDb("WriteLog_UserLog_CasinoAPI", 600))
            {
                db.AssignParamValues(param);
                db.ExecuteNonQuery();
            }
        }
    }
}