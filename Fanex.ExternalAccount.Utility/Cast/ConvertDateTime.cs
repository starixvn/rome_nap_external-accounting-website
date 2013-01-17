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
/// Summary description for Cast to DateTime
/// Author:Thai.tran
/// </summary>

namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        #region DateTime

        public const string DATE_FORMAT_VI = "d/M/yyyy";

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTime(object value)
        {
            return AsDateTime(value, DateTime.Now, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTime(object value, DateTime defaultValue)
        {
            return AsDateTime(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTime(object value, IFormatProvider provider)
        {
            return AsDateTime(value, DateTime.Now, provider);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTime(object value, DateTime defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            DateTime retval;

            try
            {
                retval = Convert.ToDateTime(value, provider);
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
        /// Returns an object as a DateTime using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static DateTime AsDateTimeInvariant(object value)
        {
            return AsDateTime(value, DateTime.Now, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static DateTime AsDateTimeInvariant(object value, DateTime defaultValue)
        {
            return AsDateTime(value, defaultValue, CultureInfo.InvariantCulture);
        }

        #endregion DateTime

        #region DateTimeWithFormat

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormat(object value, string format)
        {
            return AsDateTimeWithFormat(value, format, DateTime.Now, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormat(object value, string format, DateTime defaultValue)
        {
            return AsDateTimeWithFormat(value, format, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormat(object value, string format, IFormatProvider provider)
        {
            return AsDateTimeWithFormat(value, format, DateTime.Now, provider);
        }

        /// <summary>
        /// Returns an object as a DateTime.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormat(object value, string format, DateTime defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            DateTime retval;

            try
            {
                retval = DateTime.ParseExact(AsString(value, provider), format, provider);
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
        /// Returns an object as a DateTime using invariant culture.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormatInvariant(object value, string format)
        {
            return AsDateTimeWithFormat(value, format, DateTime.Now, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a DateTime using invariant culture.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>DateTime</returns>
        public static DateTime AsDateTimeWithFormatInvariant(object value, string format, DateTime defaultValue)
        {
            return AsDateTimeWithFormat(value, format, defaultValue, CultureInfo.InvariantCulture);
        }

        #endregion DateTimeWithFormat

        public static DateTime ResetStartDate(DateTime dt)
        {
            return dt.AddMilliseconds(-(((dt.Hour * 60 + dt.Minute) * 60 + dt.Second) * 1000 + dt.Millisecond));
        }

        public static DateTime ResetEndDate(DateTime dt)
        {
            return ResetStartDate(dt).Add(new TimeSpan(23, 59, 59));
        }
    }
}