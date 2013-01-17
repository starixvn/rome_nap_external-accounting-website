using System;
using System.Globalization;
using System.Web;

/*
 * Created 20100428@Lee - Formater functions
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Utilities
{
    public abstract class CheckRequest
    {
        protected DateTime MIN_REPORT_DATE = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
        protected DateTime MAX_REPORT_DATE = DateTime.Today.AddDays(7);

        public static int GetIntRequest(String name, int defaultValue)
        {
            HttpRequest Request = HttpContext.Current.Request;
            if (String.IsNullOrEmpty(Request[name]))
            {
                return defaultValue;
            }
            else
            {
                int value = 0;
                if (!int.TryParse(Request[name], out value)) return defaultValue;
                return value;
            }
        }

        public static DateTime GetDateTimeRequest(string name, DateTime defaultValue)
        {
            HttpRequest Request = HttpContext.Current.Request;
            if (String.IsNullOrEmpty(Request[name]))
            {
                return defaultValue;
            }
            else
            {
                DateTime value = DateTime.Now;
                if (!DateTime.TryParse(Request[name], out value)) return defaultValue;
                return value;
            }
        }

        public static bool GetBooleanRequest(String name, bool defaultValue)
        {
            HttpRequest Request = HttpContext.Current.Request;
            if (String.IsNullOrEmpty(Request[name])) return defaultValue;
            else
            {
                return Request[name].Equals("1") || Request[name].Equals("true") || Request[name].Equals("on") ? true : false;
            }
        }

        public static string GetStringRequest(String name, String defaultValue)
        {
            HttpRequest Request = HttpContext.Current.Request;
            if (String.IsNullOrEmpty(Request[name])) return defaultValue;
            else return Request[name];
        }
    }
}