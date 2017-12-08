namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Globalization;
    using System.Web;

    /// <summary>
    /// Utilities class.
    /// </summary>
    public abstract class CheckRequest
    {
        private DateTime _minReportDate = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
        private DateTime _maxReportDate = DateTime.Today.AddDays(7);

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

        /// <summary>
        /// Gets integer request.
        /// </summary>
        /// <param name="name">Query string name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Integer object.</returns>
        public static int GetIntRequest(string name, int defaultValue)
        {
            HttpRequest request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }
            else
            {
                int value = 0;
                if (!int.TryParse(request[name], out value))
                {
                    return defaultValue;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets integer request.
        /// </summary>
        /// <param name="name">Query string name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>DateTime object.</returns>
        public static DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            HttpRequest request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }
            else
            {
                DateTime value = DateTime.Now;
                if (!DateTime.TryParse(request[name], out value))
                {
                    return defaultValue;
                }

                return value;
            }
        }

        /// <summary>
        /// Gets integer request.
        /// </summary>
        /// <param name="name">Query string name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>Boolean object.</returns>
        public static bool GetBooleanRequest(string name, bool defaultValue)
        {
            HttpRequest request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }
            else
            {
                return request[name].Equals("1") || request[name].Equals("true") || request[name].Equals("on") ? true : false;
            }
        }

        /// <summary>
        /// Gets integer request.
        /// </summary>
        /// <param name="name">Query string name.</param>
        /// <param name="defaultValue">Default value.</param>
        /// <returns>String object.</returns>
        public static string GetStringRequest(string name, string defaultValue)
        {
            HttpRequest request = HttpContext.Current.Request;
            if (string.IsNullOrEmpty(request[name]))
            {
                return defaultValue;
            }
            else
            {
                return request[name];
            }
        }
    }
}