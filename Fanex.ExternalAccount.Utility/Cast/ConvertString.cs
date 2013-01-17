using System;
using System.Configuration;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

/// <summary>
/// Summary description for Cast to String
/// Author:Thai.tran
/// </summary>
namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        /// <summary>
        /// Converts an object to a String.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>string</returns>
        public static String AsString(object value)
        {
            return AsString(value, string.Empty, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a String.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>string</returns>
        public static String AsString(object value, string defaultValue)
        {
            return AsString(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a String.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns></returns>
        public static String AsString(object value, IFormatProvider provider)
        {
            return AsString(value, string.Empty, provider);
        }

        /// <summary>
        /// Returns an object as a String.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>string</returns>
        public static String AsString(object value, string defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            String retval;

            try
            {
                retval = Convert.ToString(value, provider);
            }
            catch (InvalidCastException)
            {
                retval = defaultValue;
            }
            return retval;
        }

        /// <summary>
        /// Returns an object as a String using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static String AsStringInvariant(object value)
        {
            return AsString(value, string.Empty, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a String using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static String AsStringInvariant(object value, string defaultValue)
        {
            return AsString(value, defaultValue, CultureInfo.InvariantCulture);
        }
    }
}