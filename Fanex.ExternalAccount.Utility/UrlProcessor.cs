/*
 * Created 20100603@Lee - Url processor
 * Revision:
 * ?@? - ...
 *
 */

namespace Fanex.ExternalAccount.Utilities
{
    public class UrlProcessor
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="paramName"></param>
        /// <param name="paramValue">=null to remove this param on url</param>
        /// <param name="url"></param>
        public static void SetParameterValue(string name, string value, ref string url)
        {
            string newParameter = (value != null) ? (name + "=" + value) : null;

            if (!url.Contains("?"))
            {
                url = (newParameter == null) ? url : (url + "?" + newParameter);
                return;
            }

            // Already contain ?
            string separator = url.Contains("?" + name + "=") ? "?" : "&";

            if (!url.Contains("&" + name + "=") && !url.Contains("?" + name + "="))
            {
                url = (newParameter == null) ? url : (url + "&" + newParameter);
                return;
            }
            else
            {
                int i1 = url.IndexOf(separator + name + "=");

                string tmp = url.Substring(i1);
                int i2 = tmp.IndexOf("&", 1);

                string oldParameter = i2 >= 0 ? url.Substring(i1, i2) : tmp;

                url = url.Replace(oldParameter, (newParameter == null) ? null : (separator + newParameter));
            }
        }

        public static string GetParameterValue(string name, string url)
        {
            if (url == null) return null;

            string separator = url.Contains("?" + name + "=") ? "?" : "&";

            string parameter = separator + name + "=";

            int i1 = url.IndexOf(parameter);
            if (i1 == -1) return null;

            string tmp = url.Substring(i1);
            int i2 = tmp.IndexOf("&", 1);

            string value = i2 >= 0 ? tmp.Substring(parameter.Length, i2 - parameter.Length) : tmp.Remove(0, parameter.Length);

            return value;
        }
    }
}