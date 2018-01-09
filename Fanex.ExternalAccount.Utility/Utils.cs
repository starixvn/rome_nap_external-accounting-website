// --------------------------------------------------------------------------
// <copyright file="Utils.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Data;
    using System.Web;
    using System.Web.Optimization;

    /// <summary>
    /// Utilities class for converting value types.
    /// </summary>
    public static class Utils
    {
        public enum TruncateOptions
        {
            /// <summary>
            /// None option.
            /// </summary>
            None = 0x0,

            /// <summary>
            /// Finish word option.
            /// </summary>
            FinishWord = 0x1,

            /// <summary>
            /// Allow last word to goes over max length.
            /// </summary>
            AllowLastWordToGoOverMaxLength = 0x2,

            /// <summary>
            /// Include ellipsis.
            /// </summary>
            IncludeEllipsis = 0x4
        }

        /// <summary>
        /// Formats date time.
        /// </summary>
        /// <param name="dt">Value to format.</param>
        /// <returns>String object.</returns>
        public static string FormatDateOptionalTime(object dt)
        {
            if (Convert.IsDBNull(dt) || dt == null)
            {
                return string.Empty;
            }

            try
            {
                DateTime dt2 = Convert.ToDateTime(dt);
                if ((dt2.Hour == 12 || dt2.Hour == 0) && dt2.Minute == 0)
                {
                    return dt2.ToString("MM/dd/yyyy");
                }
                else
                {
                    return dt2.ToString("MM/dd/yyyy hh:mm:ss tt");
                }
            }
            catch
            {
                return "Error";
            }
        }

        public static string ResolveCheckBox(bool isChecked)
        {
            return isChecked ? "active" : "inactive";
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static byte ToByte(object value)
        {
            byte val = 0;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return val;
            }

            byte.TryParse(value.ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static byte ToByte(this IDataReader obj, string fieldName)
        {
            byte val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return val;
            }

            byte.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static byte ToByte(this DataRow obj, string fieldName)
        {
            byte val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return val;
            }

            byte.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static short ToInt16(object value)
        {
            short val = 0;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return val;
            }

            short.TryParse(value.ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static short ToInt16(this IDataReader obj, string fieldName)
        {
            short val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return val;
            }

            short.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static short ToInt16(this DataRow obj, string fieldName)
        {
            short val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return val;
            }

            short.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static decimal ToDecimal(object value)
        {
            var val = 0.0m;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return 0.0m;
            }

            decimal.TryParse(value.ToString(), out val);

            return val.Format(2) == -0.00m ? 0.0m : val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static decimal ToDecimal(this IDataReader obj, string fieldName)
        {
            var val = 0.0m;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0.0m;
            }

            decimal.TryParse(obj[fieldName].ToString(), out val);

            return val.Format(2) == -0.00m ? 0.0m : val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static decimal ToDecimal(this DataRow obj, string fieldName)
        {
            var val = 0.0m;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0.0m;
            }

            decimal.TryParse(obj[fieldName].ToString(), out val);

            return val.Format(2) == -0.00m ? 0.0m : val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static bool ToBoolean(object value)
        {
            var val = false;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return false;
            }

            if (value.ToString() == "1")
            {
                return true;
            }

            bool.TryParse(value.ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static bool ToBoolean(this IDataReader obj, string fieldName)
        {
            var val = false;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return false;
            }

            bool.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static bool ToBoolean(this DataRow obj, string fieldName)
        {
            var val = false;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return false;
            }

            if (obj[fieldName].ToString() == "1")
            {
                return true;
            }

            bool.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static string ToString(this IDataReader obj, string fieldName)
        {
            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(obj[fieldName]);
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static string ToString(this DataRow obj, string fieldName)
        {
            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(obj[fieldName]);
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static double ToDouble(object value)
        {
            var val = 0.0d;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return 0.0d;
            }

            double.TryParse(value.ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static double ToDouble(this IDataReader obj, string fieldName)
        {
            var val = 0.0d;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0.0d;
            }

            double.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static double ToDouble(this DataRow obj, string fieldName)
        {
            var val = 0.0d;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0.0d;
            }

            double.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static DateTime ToDateTime(object value)
        {
            var val = DateTime.MinValue;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return DateTime.MinValue;
            }

            DateTime.TryParse(value.ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static DateTime ToDateTime(this IDataReader obj, string fieldName)
        {
            var val = DateTime.MinValue;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return DateTime.MinValue;
            }

            DateTime.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static DateTime ToDateTime(this DataRow obj, string fieldName)
        {
            var val = DateTime.MinValue;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return DateTime.MinValue;
            }

            DateTime.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static int ToInt32(object value)
        {
            var val = 0;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return 0;
            }

            if (value is decimal || value is double)
            {
                val = Convert.ToInt32(value);
            }
            else
            {
                int.TryParse(value.ToString(), out val);
            }

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static int ToInt32(this IDataReader obj, string fieldName)
        {
            var val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0;
            }

            if (obj[fieldName] is decimal || obj[fieldName] is double)
            {
                val = Convert.ToInt32(obj[fieldName]);
            }
            else
            {
                int.TryParse(obj[fieldName].ToString(), out val);
            }

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static int ToInt32(this DataRow obj, string fieldName)
        {
            var val = 0;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0;
            }

            if (obj[fieldName] is decimal || obj[fieldName] is double)
            {
                val = Convert.ToInt32(obj[fieldName]);
            }
            else
            {
                int.TryParse(obj[fieldName].ToString(), out val);
            }

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="value">Value to convert.</param>
        /// <returns>DataType structure or object.</returns>
        public static long ToInt64(object value)
        {
            var val = 0L;

            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return 0L;
            }

            if (value is decimal || value is double)
            {
                val = Convert.ToInt64(value);
            }
            else
            {
                long.TryParse(value.ToString(), out val);
            }

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static long ToInt64(this IDataReader obj, string fieldName)
        {
            var val = 0L;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0L;
            }

            if (obj[fieldName] is decimal || obj[fieldName] is double)
            {
                val = Convert.ToInt64(obj[fieldName]);
            }
            else
            {
                long.TryParse(obj[fieldName].ToString(), out val);
            }

            return val;
        }

        /// <summary>
        /// Convert value.
        /// </summary>
        /// <param name="obj">Object to convert.</param>
        /// <param name="fieldName">Field name.</param>
        /// <returns>DataType structure or object.</returns>
        public static long ToInt64(this DataRow obj, string fieldName)
        {
            var val = 0L;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return 0L;
            }

            if (obj[fieldName] is decimal || obj[fieldName] is double)
            {
                val = Convert.ToInt64(obj[fieldName]);
            }
            else
            {
                long.TryParse(obj[fieldName].ToString(), out val);
            }

            return val;
        }

        public static decimal Format(object value, int prec = 2)
        {
            var val = Utils.ToDecimal(value);

            return decimal.Round(val, prec);
        }

        public static string ToStringFormat(object value, int prec = 2)
        {
            var val = Utils.ToDecimal(value);

            var ret = decimal.Round(val, prec);

            return ret.ToString("#,##0.########");
        }

        public static decimal Format(this decimal obj, int prec = 2)
        {
            return decimal.Round(obj, prec);
        }

        public static string ToStringFormat(this decimal obj, int prec = 2)
        {
            var ret = decimal.Round(obj, prec);
            var valRet = ret.ToString("#,##0.########");

            return ret < 0.0m ? "<font color=\"red\">" + valRet + "</font>" : valRet;
        }

        public static decimal Format(this IDataReader obj, string fieldName, int prec = 2)
        {
            var val = Utils.ToDecimal(obj[fieldName]);

            return decimal.Round(val, prec);
        }

        public static decimal Format(this DataRow obj, string fieldName, int prec = 2)
        {
            var val = Utils.ToDecimal(obj[fieldName]);

            return decimal.Round(val, prec);
        }

        public static string ToStringFormat(this IDataReader obj, string fieldName, int prec = 2)
        {
            var val = Utils.ToDecimal(obj[fieldName]);

            var ret = decimal.Round(val, prec);

            var valRet = ret.ToString("#,##0.########");

            return ret < 0.0m ? "<font color=\"red\">" + valRet + "</font>" : valRet;
        }

        public static string ToStringFormat(this DataRow obj, string fieldName, int prec = 2)
        {
            var val = Utils.ToDecimal(obj[fieldName]);

            var ret = decimal.Round(val, prec);

            var valRet = ret.ToString("#,##0.########");

            return ret < 0.0m ? "<font color=\"red\">" + valRet + "</font>" : valRet;
        }

        public static string DecodeHtml(this DataRow obj, string fieldName)
        {
            return HttpUtility.HtmlDecode(obj.ToString(fieldName));
        }

        public static string EncodeHtml(this DataRow obj, string fieldName)
        {
            return HttpUtility.HtmlEncode(obj.ToString(fieldName));
        }

        public static string DecodeHtml(this string str)
        {
            return HttpUtility.HtmlDecode(str);
        }

        public static string EncodeHtml(this string str)
        {
            return HttpUtility.HtmlEncode(str);
        }

        public static string EncodeHtml(this object obj)
        {
            return HttpUtility.HtmlEncode(obj);
        }

        /// <summary>
        /// Truncates a string to a certain length.
        /// </summary>
        /// <param name="valueToTruncate">Value to truncate.</param>
        /// <param name="maxLength">Maximum length that the string should allow.</param>
        /// <param name="options">Option flags to determine how to truncate the string.</param>
        /// <returns>Truncated string.</returns>
        public static string TruncateString(string valueToTruncate, int maxLength, TruncateOptions options)
        {
            if (valueToTruncate == null || maxLength <= 0)
            {
                return string.Empty;
            }

            if (valueToTruncate.Length <= maxLength)
            {
                return valueToTruncate;
            }

            bool includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
            bool finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
            bool allowLastWordOverflow = (options & TruncateOptions.AllowLastWordToGoOverMaxLength) == TruncateOptions.AllowLastWordToGoOverMaxLength;

            string retValue = valueToTruncate;

            if (includeEllipsis)
            {
                maxLength -= 1;
            }

            int lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

            if (!finishWord)
            {
                retValue = retValue.Remove(maxLength);
            }
            else if (allowLastWordOverflow)
            {
                int spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
                if (spaceIndex != -1)
                {
                    retValue = retValue.Remove(spaceIndex);
                }
            }
            else if (lastSpaceIndex > -1)
            {
                retValue = retValue.Remove(lastSpaceIndex);
            }

            if (includeEllipsis && retValue.Length < valueToTruncate.Length)
            {
                retValue += "...";
            }

            return retValue;
        }

        public static string ToStringExt(this DateTime date, string format = "MM/dd/yyyy HH:mm:ss tt")
        {
            if (date.Equals(DateTime.MinValue))
            {
                return string.Empty;
            }

            return date.ToString(format);
        }
    }
}