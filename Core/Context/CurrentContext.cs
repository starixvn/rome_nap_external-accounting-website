namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text.RegularExpressions;
    using System.Web;
    using Common;
    using CustomerSetting.IP.Utils;
    using CustomerSetting.Module.Core.Web;

    public class CurrentContext : ICurrentContext
    {
        private readonly string[] custIdParamNames =
            {"custId", "customerId", "parentId", "srecommend", "mrecommend", "recommend", "agentId"};

        private readonly string[] custIdsNames = { "custIds", "customerIds" };

        public static string LoginErrorMessage { get; set; } = string.Empty;

        public static ICurrentContext Instance => new CurrentContext();

        public int CustId
        {
            get
            {
                var custId = GetCustomerIdInQueryString();

                if (custId <= 0)
                {
                    custId = GetCustomerIdInJson();
                }

                return custId;
            }
        }

        public IEnumerable<int> CustIds => GetCustomerIdsInQueryString();

        public static string GroupName => HttpContext.Current.Request["groupName"];

        public HttpContext Context => HttpContext.Current;

        public string Ip4Address
        {
            get
            {
                var ip = string.Empty;

                if (HttpContext.Current.Request != null)
                {
                    foreach (var ipa in Dns.GetHostAddresses(HttpContext.Current.Request.UserHostAddress))
                    {
                        if (ipa.AddressFamily.ToString().Equals("InterNetwork"))
                        {
                            ip = ipa.ToString();
                            break;
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(ip))
                {
                    return ip;
                }

                foreach (var ipa in Dns.GetHostAddresses(Dns.GetHostName()))
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
                var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }
        }

        public string DomainName => HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        public string FullDomainName
        {
            get
            {
                var variables = HttpContext.Current.Request.ServerVariables;

                var protocol =
                    variables["SERVER_PORT_SECURE"] == "1" ? "https://" : "http://"; //DevSkim: ignore DS137138
                var domain = variables["SERVER_NAME"];
                var port = variables["SERVER_PORT"];

                return protocol + domain + ":" + port;
            }
        }

        public string CountryCode
        {
            get
            {
                var regionInfo = ResolveCountry();

                return regionInfo?.EnglishName;
            }
        }

        private RegionInfo ResolveCountry()
        {
            try
            {
                return new RegionInfo(IpUtils.LocationFromIP(Ip4Address).countrySHORT);
            }
            catch
            {
                return null;
            }
        }

        private int GetCustomerIdInQueryString()
        {
            var request = HttpContext.Current.Request;

            foreach (var custIdParamName in custIdParamNames)
            {
                var custId = Converter.ToInt(request[custIdParamName]);

                if (custId > 0)
                {
                    return custId;
                }
            }

            return 0;
        }

        private IEnumerable<int> GetCustomerIdsInQueryString()
        {
            var request = HttpContext.Current.Request;
            foreach (var custIdName in custIdsNames)
            {
                var custIds = request[custIdName];
                if (!string.IsNullOrEmpty(custIds))
                {
                    return custIds.Split(',').Select(id => Converter.ToInt(id));
                }
            }

            return Enumerable.Empty<int>();
        }

        private int GetCustomerIdInJson()
        {            
            foreach (var custIdName in custIdParamNames)
            {
                var custId = GetPropertyValueInJson(custIdName);
                if (custId > 0)
                {
                    return custId;
                }
            }

            return 0;
        }

        private static int GetPropertyValueInJson(string propertyName)
        {
            var propertyValue = 0;
            if (string.IsNullOrEmpty(propertyName))
            {
                return propertyValue;
            }

            var jsonString = GetJsonDataInRequestBody(HttpContext.Current.Request.InputStream);
            var pattern = $"(\"{propertyName}\":)(?<paramValue>[^,}}]+)";
            var regexMatch = Regex.Match(jsonString, pattern, RegexOptions.IgnoreCase);
            if (regexMatch.Success)
            {
                propertyValue = Convert.ToInt32(regexMatch.Groups["paramValue"].ToString().Replace("\"", string.Empty));
            }

            return propertyValue;
        }

        private static string GetJsonDataInRequestBody(Stream requestStream)
        {
            if (requestStream == null)
            {
                return string.Empty;
            }

            requestStream.Seek(0, SeekOrigin.Begin);
            var bodyData = new StreamReader(requestStream).ReadToEnd();
            requestStream.Seek(0, SeekOrigin.Begin);
            if (!string.IsNullOrWhiteSpace(bodyData)
                && bodyData.Contains("{"))
            {
                return HttpUtility.UrlDecode(bodyData).Replace(@"\", string.Empty);
            }

            return string.Empty;
        }
    }
}