//----------------------------------------------------------------------------------
// File: CastAide.Decimal.cs
//
// Copyright (c) 2007 Ste Brennan (dnaide.com)
// Licensed under the COMMON DEVELOPMENT AND DISTRIBUTION LICENSE (CDDL) Version 1.0
// http://www.opensource.org/licenses/cddl1.php
//
//----------------------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

/// <summary>
/// Summary description for Cast to Decimal
/// Author:Thai.tran
/// </summary>
namespace Fanex.ExternalAccount.Utilities
{
    public static partial class Cast
    {
        /// <summary>
        /// Returns an object as a Decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <returns>Decimal</returns>
        public static Decimal AsDecimal(object value)
        {
            return AsDecimal(value, 0, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <returns>Decimal</returns>
        public static Decimal AsDecimal(object value, Decimal defaultValue)
        {
            return AsDecimal(value, defaultValue, CultureInfo.CurrentCulture);
        }

        /// <summary>
        /// Returns an object as a Decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Decimal</returns>
        public static Decimal AsDecimal(object value, IFormatProvider provider)
        {
            return AsDecimal(value, 0, provider);
        }

        /// <summary>
        /// Returns an object as a Decimal.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue">Value returned if cast fails</param>
        /// <param name="provider">Instance of IFormatProvider</param>
        /// <returns>Decimal</returns>
        public static Decimal AsDecimal(object value, Decimal defaultValue, IFormatProvider provider)
        {
            if (value == null || value == System.DBNull.Value)
                return defaultValue;

            Decimal retval;

            if (provider == null)
                provider = CultureInfo.CurrentCulture;

            try
            {
                retval = Convert.ToDecimal(value, provider);
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
        /// Returns an object as a Decimal using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static Decimal AsDecimalInvariant(object value)
        {
            return AsDecimal(value, 0, CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Returns an object as a Decimal using invariant culture
        /// </summary>
        /// <param name="value"></param>
        /// <param name="defaultValue"></param>
        /// <returns></returns>
        public static Decimal AsDecimalInvariant(object value, Decimal defaultValue)
        {
            return AsDecimal(value, defaultValue, CultureInfo.InvariantCulture);
        }
    }
}