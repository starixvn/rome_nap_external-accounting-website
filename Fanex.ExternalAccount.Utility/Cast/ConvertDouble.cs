using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        /// <summary>
        /// Returns an object as a Double.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Double</returns>
        public static Double AsDouble(object value)
        {
            return AsDouble(value, 0, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>Double</returns>
        public static Double AsDouble(object value, Double defaultValue)
        {
            return AsDouble(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Double</returns>
        public static Double AsDouble(object value, IFormatProvider provider)
        {
            return AsDouble(value, 0, provider);
        }

        /// <summary>
        /// Returns an object as a Double.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Double</returns>
        public static Double AsDouble(object value, Double defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            Double retval;

            try
            {
                retval = Convert.ToDouble(value, provider);
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
        /// Returns an object as a Double using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Double AsDoubleInvariant(object value)
        {
            return AsDouble(value, 0, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a Double using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Double AsDoubleInvariant(object value, Double defaultValue)
        {
            return AsDouble(value, defaultValue, CultureInfo.InvariantCulture);
        }
    }
}