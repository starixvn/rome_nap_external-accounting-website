namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Web;
    using System.Linq;

    public abstract class CheckRequest
    {
        DateTime _minReportDate = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
        DateTime _maxReportDate = DateTime.Today.AddDays(7);

        protected DateTime MinReportDate
        {
            get { return _minReportDate; }
            set { _minReportDate = value; }
        }

        protected DateTime MaxReportDate
        {
            get { return _maxReportDate; }
            set { _maxReportDate = value; }
        }

        public static int GetIntRequest(string name, int defaultValue)
        {
            var request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }

            int value;

            return !int.TryParse(request[name], out value) ? defaultValue : value;
        }

        public static DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            var request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }

            DateTime value;

            return !DateTime.TryParse(request[name], out value) ? defaultValue : value;
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