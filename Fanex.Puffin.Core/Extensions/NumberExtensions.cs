// --------------------------------------------------------------------------
// <copyright file="NumberExtensions.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.11.6">Created</change>
// </history>

using System;

namespace Fanex.Puffin.Core.Extensions
{
    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public static class NumberExtensions
    {
        /// <summary>
        /// Format int
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Format(this int value)
        {
            return DoFormat(value, 0);
        }

        /// <summary>
        /// Format Object
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Format(this object value)
        {
            return DoFormat(value, 0);
        }

        /// <summary>
        /// Format Decimal Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns></returns>
        public static string Format(this decimal value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Format Object value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns></returns>
        public static string Format(this object value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Format Double Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns></returns>
        public static string Format(this double value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Format Float Value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns></returns>
        public static string Format(this float value, byte decDigits)
        {
            return DoFormat(value, decDigits);
        }

        /// <summary>
        /// Format Double Value Quotation mark
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns></returns>
        public static string FormatNegative(this double value, byte decDigits)
        {
            var res = string.Empty;
            if (value < 0)
            {
                value = -value;
                res = DoFormat(value, decDigits);
                res = "(" + res + ")";
            }
            else
            {
                res = DoFormat(value, decDigits);
            }

            return res;
        }

        /// <summary>
        /// Format specified number value
        /// </summary>
        /// <param name="value"></param>
        /// <param name="decDigits"></param>
        /// <returns>Value after rounding and formatting</returns>
        private static string DoFormat(object value, byte decDigits)
        {
            if (value != null)
            {
                string formatString = "{0:n" + decDigits.ToString() + "}";
                string result = null;
                if (value != null)
                {
                    result = string.Format(formatString, value);
                }

                return result;
            }
            else
            {
                throw new NullReferenceException();
            }
        }
    }
}
