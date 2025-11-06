namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Web;
    using System.Linq;
    using Microsoft.AspNetCore.Http;

    public abstract class CheckRequest
    {
        private static IHttpContextAccessor _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

#pragma warning disable S109 // Magic numbers should not be used
        protected DateTime MinReportDate { get; set; } = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);

        protected DateTime MaxReportDate { get; set; } = DateTime.Today.AddDays(7);
#pragma warning restore S109 // Magic numbers should not be used

        public static int GetIntRequest(string name, int defaultValue)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string value = request.Query[name].FirstOrDefault() ?? request.Form[name].FirstOrDefault();
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return !int.TryParse(value, out int result) ? defaultValue : result;
        }

        public static DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string value = request.Query[name].FirstOrDefault() ?? request.Form[name].FirstOrDefault();
            if (string.IsNullOrEmpty(value))
            {
                return defaultValue;
            }

            return !DateTime.TryParse(value, out DateTime result) ? defaultValue : result;
        }

        public static bool GetBooleanRequest(string name, bool defaultValue)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            var requestName = request.Query[name].FirstOrDefault() ?? request.Form[name].FirstOrDefault();

            if (string.IsNullOrEmpty(requestName))
            {
                return defaultValue;
            }

            var enableFlags = new[] { "1", "true", "on" };

            return enableFlags.Any(str => requestName.Equals(str));
        }

        public static string GetStringRequest(string name, string defaultValue)
        {
            var request = _httpContextAccessor.HttpContext.Request;
            string value = request.Query[name].FirstOrDefault() ?? request.Form[name].FirstOrDefault();

            return string.IsNullOrEmpty(value) ? defaultValue : value;
        }
    }
}

