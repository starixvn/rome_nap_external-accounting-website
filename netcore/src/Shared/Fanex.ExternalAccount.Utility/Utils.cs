namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Net;
    using System.Web;

#pragma warning disable S109 // Magic numbers should not be used

    public static class Utils
    {
        public static bool IsValidIpAddress(string ip)
        {
            return IPAddress.TryParse(ip, out IPAddress address) && !ip.Equals("::1", StringComparison.OrdinalIgnoreCase);
        }

        public static string FormatDateOptionalTime(object dt)
        {
            if (Convert.IsDBNull(dt) || dt == null)
            {
                return string.Empty;
            }

            try
            {
                var dt2 = Convert.ToDateTime(dt);

                return (dt2.Hour == 12 || dt2.Hour == 0) && dt2.Minute == 0
                    ? dt2.ToString("MM/dd/yyyy")
                    : dt2.ToString("MM/dd/yyyy hh:mm:ss tt");
            }
            catch
            {
                return "Error";
            }
        }

        public static decimal ToDecimal(object value)
        {
            if (IsEmptyData(value))
            {
                return 0.0m;
            }

            decimal.TryParse(value.ToString(), out decimal val);

            return val;
        }

        public static decimal ToDecimal(this IDataReader obj, string fieldName)
            => ToDecimal(obj[fieldName]);

        public static bool ToBoolean(object value, bool defaultValue = false)
        {
            if (IsEmptyData(value))
            {
                return defaultValue;
            }

            if (value.ToString() == "1")
            {
                return true;
            }

            bool.TryParse(value.ToString(), out bool val);
            return val;
        }

        public static bool ToBoolean(this IDataReader obj, string fieldName, bool defaultValue = false)
        {
            if (IsEmptyData(obj[fieldName]))
            {
                return defaultValue;
            }

            bool.TryParse(obj[fieldName].ToString(), out bool val);
            return val;
        }

        public static bool ToBoolean(this DataRow obj, string fieldName, bool defaultValue = false)
            => ToBoolean(obj.Table.Columns.Contains(fieldName)
                ? obj[fieldName]
                : null,
                defaultValue);

        public static string ToString(object value)
            => IsEmptyData(value) ? string.Empty : Convert.ToString(value);

        public static string ToString(this IDataReader obj, string fieldName)
            => ToString(obj[fieldName]);

        public static string ToString(this DataRow obj, string fieldName)
            => ToString(obj[fieldName]);

        public static string JoinToString<T>(IEnumerable<T> list, string separator = ",")
            => list == null || !list.Any() ? string.Empty : string.Join(separator, list);

        public static double ToDouble(object value)
        {
            if (IsEmptyData(value))
            {
                return 0.0d;
            }

            double.TryParse(value.ToString(), out double val);

            return val;
        }

        public static DateTime ToDateTime(object value)
        {
            if (IsEmptyData(value))
            {
                return DateTime.MinValue;
            }

            DateTime.TryParse(value.ToString(), out DateTime val);
            return val;
        }

        public static DateTime? ToNullAbleDateTime(object value)
        {
            if (IsEmptyData(value))
            {
                return null;
            }

            DateTime.TryParse(value.ToString(), out DateTime val);
            return val;
        }

        public static string ToStringFormat(this decimal obj, int prec = 2)
        {
            var ret = decimal.Round(obj, prec);
            var valRet = ret.ToString("#,##0.########");

            return ret < 0.0m ? "<font color=\"red\">" + valRet + "</font>" : valRet;
        }

        public static byte ToByte(object value)
        {
            if (IsEmptyData(value))
            {
                return 0;
            }

            byte val;

            if (value is decimal || value is double)
            {
                val = Convert.ToByte(value);
            }
            else if (value is bool)
            {
                val = (bool)value ? (byte)1 : (byte)0;
            }
            else
            {
                byte.TryParse(value.ToString(), out val);
            }

            return val;
        }

        public static int ToInt32(object value)
        {
            if (IsEmptyData(value))
            {
                return 0;
            }

            int val;

            if (value is decimal || value is double)
            {
                val = Convert.ToInt32(value);
            }
            else if (value is bool)
            {
                val = (bool)value ? 1 : 0;
            }
            else
            {
                int.TryParse(value.ToString(), out val);
            }

            return val;
        }

        public static int ToInt32(this IDataReader obj, string fieldName)
            => ToInt32(obj[fieldName]);

        public static int ToInt32(this DataRow obj, string fieldName)
            => ToInt32(obj[fieldName]);

        public static decimal Format(this decimal obj, int prec = 2)
            => decimal.Round(obj, prec);

        public static string DecodeHtml(this string str)
            => HttpUtility.HtmlDecode(str);

        public static string DecodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlDecode(obj.ToString(fieldName));

        public static string EncodeHtml(this string str)
            => HttpUtility.HtmlEncode(str);

        public static string EncodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlEncode(obj.ToString(fieldName));

        public static string ToStringExt(this DateTime date, string format = "MM/dd/yyyy HH:mm:ss tt")
            => date.Equals(DateTime.MinValue) ? string.Empty : date.ToString(format);

        private static bool IsEmptyData(object value) =>
            value == null || value == DBNull.Value || value.ToString()?.Length == 0;

        public static bool HasColumn(IDataRecord dataReader, string columnName)
        {
            for (int i = 0; i < dataReader.FieldCount; i++)
            {
                if (dataReader.GetName(i).Equals(columnName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }
            return false;
        }

        public static double ConvertDateTimeToMilliseconds(DateTime dateTime) => (dateTime - DateTime.MinValue).TotalMilliseconds;

        public static IEnumerable<int> ToEnumerableInt(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var arrayString = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in arrayString)
                {
                    if (int.TryParse(item, out var intValue))
                    {
                        yield return intValue;
                    }
                }
            }
        }

        public static string GetDaySuffix(int day)
        {
            switch (day)
            {
                case 1:
                case 21:
                case 31:
                    return "st";
                case 2:
                case 22:
                    return "nd";
                case 3:
                case 23:
                    return "rd";
                default:
                    return "th";
            }
        }
    }

#pragma warning restore S109 // Magic numbers should not be used
}