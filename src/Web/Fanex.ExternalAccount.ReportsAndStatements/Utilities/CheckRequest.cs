using System;

namespace Fanex.ExternalAccount.ReportsAndStatements.Utilities
{
    public abstract class CheckRequest
    {
        protected DateTime MinReportDate { get; set; } = new DateTime(DateTime.Today.AddMonths(-3).Year, DateTime.Today.AddMonths(-3).Month, 1);
        protected DateTime MaxReportDate { get; set; } = DateTime.Today.AddDays(7);

        public static int GetIntRequest(Microsoft.AspNetCore.Http.IFormCollection form, string name, int defaultValue)
        {
            if (string.IsNullOrEmpty(form[name].ToString()))
            {
                return defaultValue;
            }

            return int.TryParse(form[name], out int value) ? value : defaultValue;
        }

        public static DateTime GetDateTimeRequest(Microsoft.AspNetCore.Http.IFormCollection form, string name, DateTime defaultValue)
        {
            if (string.IsNullOrEmpty(form[name].ToString()))
            {
                return defaultValue;
            }

            return DateTime.TryParse(form[name], out DateTime value) ? value : defaultValue;
        }

        public static bool GetBooleanRequest(Microsoft.AspNetCore.Http.IFormCollection form, string name, bool defaultValue)
        {
            var requestName = form[name].ToString();

            if (string.IsNullOrEmpty(requestName))
            {
                return defaultValue;
            }

            var enableFlags = new[] { "1", "true", "on" };

            return enableFlags.Contains(requestName.ToLower());
        }

        public static string GetStringRequest(Microsoft.AspNetCore.Http.IFormCollection form, string name, string defaultValue)
        {
            return string.IsNullOrEmpty(form[name].ToString()) ? defaultValue : form[name].ToString();
        }
    }
}
