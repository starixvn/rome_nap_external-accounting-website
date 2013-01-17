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
/// Summary description for Cast to Boolean
/// Author:Thai.tran
/// </summary>
namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Boolean</returns>
        public static Boolean AsBooleanFromInt(object value)
        {
            return AsBooleanFromInt(value, false, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>Boolean</returns>
        public static Boolean AsBooleanFromInt(object value, Boolean defaultValue)
        {
            return AsBooleanFromInt(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns></returns>
        public static Boolean AsBooleanFromInt(object value, IFormatProvider provider)
        {
            return AsBooleanFromInt(value, false, provider);
        }

        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Boolean</returns>
        public static Boolean AsBooleanFromInt(object value, Boolean defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            Boolean retval;
            try
            {
                value = value.ToString().Equals("1") ? "true" : "false";
                retval = Convert.ToBoolean(value, provider);
            }
            catch (InvalidCastException)
            {
                retval = defaultValue;
            }
            catch (FormatException)
            {
                retval = defaultValue;
            }
            return retval;
        }

        /// <summary>
        /// Returns an object as a Boolean using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Boolean AsBooleanInvariant(object value)
        {
            if (value == null || false.Equals(value)) return false;
            if (true.Equals(value)) return true;

            return AsBooleanFromInt(value, false, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a Boolean using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Boolean AsBooleanInvariant(object value, Boolean defaultValue)
        {
            return AsBooleanFromInt(value, defaultValue, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>Boolean</returns>
        public static Boolean AsBooleanFromString(object value, Boolean defaultValue)
        {
            return AsBooleanFromString(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Boolean.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Boolean</returns>
        public static Boolean AsBooleanFromString(object value, Boolean defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            Boolean retval;
            try
            {
                value = value.ToString().ToLower().Equals("true") ? "true" : "false";
                retval = Convert.ToBoolean(value, provider);
            }
            catch (InvalidCastException)
            {
                retval = defaultValue;
            }
            catch (FormatException)
            {
                retval = defaultValue;
            }
            return retval;
        }
    }
}