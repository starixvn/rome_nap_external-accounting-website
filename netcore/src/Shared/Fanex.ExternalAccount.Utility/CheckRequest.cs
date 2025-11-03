namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Web;
    using System.Linq;

    public abstract class CheckRequest
    {
#pragma warning disable S109 // Magic numbers should not be used
        protected DateTime MinReportDate { get; set; } = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);

        protected DateTime MaxReportDate { get; set; } = DateTime.Today.AddDays(7);
#pragma warning restore S109 // Magic numbers should not be used

        public static int GetIntRequest(string name, int defaultValue)
        {
            var request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }

            return !int.TryParse(request[name], out int value) ? defaultValue : value;
        }

        public static DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            var request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }

            return !DateTime.TryParse(request[name], out DateTime value) ? defaultValue : value;
        }

        public static bool GetBooleanRequest(string name, bool defaultValue)
        {
            var request = HttpContext.Current.Request;
            var requestName = request[name];

            if (string.IsNullOrEmpty(requestName))
            {
                return defaultValue;
            }

            var enableFlags = new[] { "1", "true", "on" };

            return enableFlags.Any(str => requestName.Equals(str));
        }

        public static string GetStringRequest(string name, string defaultValue)
        {
            var request = HttpContext.Current.Request;

            return string.IsNullOrEmpty(request[name]) ? defaultValue : request[name];
        }
    }
}

