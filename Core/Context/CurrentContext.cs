// --------------------------------------------------------------------------
// <copyright file="CurrentContext.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Context of current session
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Core.Context
{
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Net;
    using System.Web;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.CustomerSetting.Module.Core.Web;

    /// <summary>
    /// Current context of session.
    /// </summary>
    public class CurrentContext : ICurrentContext
    {
        private static string loginErrorMessage = string.Empty;

        public static string LoginErrorMessage
        {
            get { return CurrentContext.loginErrorMessage; }
            set { CurrentContext.loginErrorMessage = value; }
        }

        public static ICurrentContext Instance
        {
            get
            {
                return new CurrentContext();
            }
        }

        public System.Web.HttpContext Context
        {
            get { return HttpContext.Current; }
        }

        public string Ip4Address
        {
            get
            {
                string ip = string.Empty;

                foreach (IPAddress ipa in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
                {
                    if (ipa.AddressFamily.ToString().Equals("InterNetwork"))
                    {
                        ip = ipa.ToString();
                        break;
                    }
                }

                if (!ip.Equals(string.Empty))
                {
                    return ip;
                }

                foreach (IPAddress ipa in Dns.GetHostAddresses(Dns.GetHostName()))
                {
                    if (ipa.AddressFamily.ToString().Equals("InterNetwork"))
                    {
                        ip = ipa.ToString();
                        break;
                    }
                }

                return ip.Trim();
            }
        }

        public string Ip6Address
        {
            get
            {
                string ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (!string.IsNullOrEmpty(ip))
                {
                    string[] range = ip.Split(',');
                    string trueIP = range[0].Trim();
                }
                else
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }
        }

        public string DomainName
        {
            get { return HttpContext.Current.Request.ServerVariables["SERVER_NAME"]; }
        }

        public string FullDomainName
        {
            get
            {
                NameValueCollection variables = HttpContext.Current.Request.ServerVariables;

                string protocol = (variables["SERVER_PORT_SECURE"] == "1") ? "https://" : "http://";
                string domain = variables["SERVER_NAME"];
                string port = variables["SERVER_PORT"];

                return protocol + domain + ":" + port;
            }
        }

        public string CountryCode
        {
            get
            {
                RegionInfo regionInfo = this.ResolveCountry();

                return regionInfo == null ? null : regionInfo.GeoId.ToString();
            }
        }
        
        private RegionInfo ResolveCountry()
        {
            return new RegionInfo(IpUtils.LocationFromIP(this.Ip4Address).countrySHORT);
        }
    }
}