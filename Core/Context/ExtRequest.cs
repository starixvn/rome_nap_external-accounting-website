namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Text.RegularExpressions;
    using System.Web;

    public interface IExtRequest
    {
        string GetGroupName(string group = null);

        int GetGroupId();

        int CustId { get; }

        string CustName { get; }

        IEnumerable<int> CustIds { get; }
    }

    public class ExtRequest : IExtRequest
    {
        private readonly string[] custIdParamNames =
            {"custId", "customerId", "parentId", "srecommend", "mrecommend", "recommend", "agentId"};

        private readonly string[] custIdsNames = { "custIds", "customerIds" };

        private readonly string[] custNameParamNames = { "username" };

        public static IExtRequest Instance => new ExtRequest();

        public string GetGroupName(string group = null)
        {
            var newGroup = string.IsNullOrEmpty(group) ? HttpContext.Current.Request["groupName"] : group;

            return string.IsNullOrEmpty(newGroup) ? CurrentSession.Instance.CurrencyGroup : newGroup;
        }

        public int GetGroupId()
        {
            var groupIdValue = HttpContext.Current.Request["groupid"];

            if (string.IsNullOrWhiteSpace(groupIdValue))
            {
                return CurrentSession.Instance.GroupId;
            }

            return int.TryParse(groupIdValue, out var groupId) ? groupId : default(int);
        }

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

        public string CustName
        {
            get
            {
                return GetCustNameInQueryString();
            }
        }

        public IEnumerable<int> CustIds => GetCustomerIdsInQueryString();

        private int GetCustomerIdInQueryString()
        {
            var request = HttpContext.Current.Request;

            foreach (var custIdParamName in custIdParamNames)
            {
                var custId = Convert.ToInt32(request[custIdParamName]);

                if (custId > 0)
                {
                    return custId;
                }
            }

            return 0;
        }

        private string GetCustNameInQueryString()
        {
            var request = HttpContext.Current.Request;

            foreach (var paramName in custNameParamNames)
            {
                var custName = request[paramName];

                if (!string.IsNullOrWhiteSpace(custName))
                {
                    return custName;
                }
            }

            return string.Empty;
        }

        private IEnumerable<int> GetCustomerIdsInQueryString()
        {
            var request = HttpContext.Current.Request;
            foreach (var custIdName in custIdsNames)
            {
                var custIds = request[custIdName];
                if (!string.IsNullOrEmpty(custIds))
                {
                    return custIds.Split(',').Select(id => Convert.ToInt32(id));
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