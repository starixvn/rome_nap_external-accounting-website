// --------------------------------------------------------------------------
// <copyright file="CurrentContext.cs" company="">
// Copyright (c) . All rights reserved.
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
    using System;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;
    using Common;
    using Fanex.CustomerSetting.IP.Utils;
    using Fanex.CustomerSetting.Module.Core.Web;

    /// <summary>
    /// Current context of session.
    /// </summary>
    public class CurrentContext : ICurrentContext
    {
        private readonly string[] custIdNames = new string[] { "custId", "customerId" };
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

                return regionInfo == null ? null : regionInfo.EnglishName;
            }
        }

        private RegionInfo ResolveCountry()
        {
            try
            {
                return new RegionInfo(IpUtils.LocationFromIP(this.Ip4Address).countrySHORT);
            }
            catch
            {
                return null;
            }
        }

        public int CustId
        {
            get
            {
                var custId = GetCustomerIdFromQueryString();

                if (custId <= 0)
                {
                    custId = GetCustomerIdFromJson();
                }

                return custId;
            }
        }

        public string GroupName
        {
            get
            {
                var groupName = HttpContext.Current.Request["groupName"];

                return groupName;
            }
        }
        private int GetCustomerIdFromQueryString()
        {
            var custId = 0;
            var i = 0;

            while (custId <= 0 && i < custIdNames.Length)
            {
                custId = Converter.ToInt(HttpContext.Current.Request[custIdNames[i]]);
                i++;
            }

            return custId;
        }

        private int GetCustomerIdFromJson()
        {
            var custId = 0;
            var i = 0;

            while (custId <= 0 && i < custIdNames.Length)
            {
                custId = GetCustomerId(custIdNames[i]);
                i++;
            }

            return custId;
        }

        private int GetCustomerId(string parameterName)
        {
            var customerId = 0;
            if (!string.IsNullOrEmpty(parameterName))
            {
                var jsonString = GetJsonDataInRequestBody(HttpContext.Current.Request.InputStream);
                var pattern = string.Format("(\"{0}\":)(?<paramValue>[^,]+)", parameterName);
                Match regexMatch = Regex.Match(jsonString, pattern, RegexOptions.IgnoreCase);
                if (regexMatch.Success)
                {
                    customerId = Convert.ToInt32(regexMatch.Groups["paramValue"].ToString().Replace("\"", string.Empty));
                }
            }

            return customerId;
        }

        private string GetJsonDataInRequestBody(Stream requestStream)
        {
            if (requestStream != null)
            {
                requestStream.Seek(0, SeekOrigin.Begin);
                var bodyData = new StreamReader(requestStream).ReadToEnd();
                requestStream.Seek(0, SeekOrigin.Begin);
                if (!string.IsNullOrWhiteSpace(bodyData)
                    && bodyData.Contains("{"))
                {
                    return HttpUtility.UrlDecode(bodyData).Replace(@"\", string.Empty);
                }
            }

            return string.Empty;
        }
    }
}