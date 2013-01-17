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
/// Summary description for Cast to Int32
/// Author:Thai.tran
/// </summary>
namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        /// <summary>
        /// Returns an object as a Int32.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Int32</returns>
        public static Int32 AsInt32(object value)
        {
            return AsInt32(value, 0, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Int32.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>Int32</returns>
        public static Int32 AsInt32(object value, Int32 defaultValue)
        {
            return AsInt32(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Int32.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Int32</returns>
        public static Int32 AsInt32(object value, IFormatProvider provider)
        {
            return AsInt32(value, 0, provider);
        }

        /// <summary>
        /// Returns an object as a Int32.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Int32</returns>
        public static Int32 AsInt32(object value, Int32 defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            Int32 retval;

            try
            {
                retval = Convert.ToInt32(value, provider);
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
        /// Returns an object as a Int32 using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Int32 AsInt32Invariant(object value)
        {
            return AsInt32(value, 0, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a Int32 using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Int32 AsInt32Invariant(object value, Int32 defaultValue)
        {
            return AsInt32(value, defaultValue, CultureInfo.InvariantCulture);
        }
    }
}