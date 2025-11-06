namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Text;
    using System.Text.RegularExpressions;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;

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

        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static HttpContext? HttpContext
        {
            get
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    return _httpContextAccessor.HttpContext;
                }
                
                // Fallback
                try
                {
                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    var accessor = serviceProvider.GetService<IHttpContextAccessor>();
                    return accessor?.HttpContext;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static IExtRequest Instance => new ExtRequest();

        public string GetGroupName(string group = null)
        {
            var request = HttpContext?.Request;
            var newGroup = string.IsNullOrEmpty(group) && request != null
                ? request.Query["groupName"].ToString()
                : group;

            return string.IsNullOrEmpty(newGroup) ? CurrentSession.Instance.CurrencyGroup : newGroup;
        }

        public int GetGroupId()
        {
            var request = HttpContext?.Request;
            var groupIdValue = request?.Query["groupid"].ToString();

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
            var request = HttpContext?.Request;
            if (request == null)
                return 0;

            foreach (var custIdParamName in custIdParamNames)
            {
                var queryValue = request.Query[custIdParamName].ToString();
                if (int.TryParse(queryValue, out var custId) && custId > 0)
                {
                    return custId;
                }
            }

            return 0;
        }

        private string GetCustNameInQueryString()
        {
            var request = HttpContext?.Request;
            if (request == null)
                return string.Empty;

            foreach (var paramName in custNameParamNames)
            {
                var custName = request.Query[paramName].ToString();

                if (!string.IsNullOrWhiteSpace(custName))
                {
                    return custName;
                }
            }

            return string.Empty;
        }

        private IEnumerable<int> GetCustomerIdsInQueryString()
        {
            var request = HttpContext?.Request;
            if (request == null)
                return Enumerable.Empty<int>();

            foreach (var custIdName in custIdsNames)
            {
                var custIds = request.Query[custIdName].ToString();
                if (!string.IsNullOrEmpty(custIds))
                {
                    return custIds.Split(',').Select(id => Convert.ToInt32(id)).Where(id => id > 0);
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

            var jsonString = GetJsonDataInRequestBody();
            var pattern = $"(\"{propertyName}\":)(?<paramValue>[^,}}]+)";
            var regexMatch = Regex.Match(jsonString, pattern, RegexOptions.IgnoreCase);
            if (regexMatch.Success)
            {
                propertyValue = Convert.ToInt32(regexMatch.Groups["paramValue"].ToString().Replace("\"", string.Empty));
            }

            return propertyValue;
        }

        private static string GetJsonDataInRequestBody()
        {
            var request = HttpContext?.Request;
            if (request?.Body == null)
            {
                return string.Empty;
            }

            try
            {
                // In ASP.NET Core, we need to enable buffering to read the body multiple times
                if (!request.Body.CanSeek)
                {
                    request.EnableBuffering();
                }

                request.Body.Position = 0;
                using var reader = new StreamReader(request.Body, Encoding.UTF8, leaveOpen: true);
                var bodyData = reader.ReadToEnd();
                request.Body.Position = 0;

                if (!string.IsNullOrWhiteSpace(bodyData)
                    && bodyData.Contains("{"))
                {
                    return WebUtility.UrlDecode(bodyData).Replace(@"\", string.Empty);
                }

                return string.Empty;
            }
            catch
            {
                return string.Empty;
            }
        }
    }
}
