namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Data;
    using System.Web;
    using System.Web.Optimization;

    public static class Utils
    {
        [Flags]
        public enum TruncateOptions
        {
            None = 0x0,

            FinishWord = 0x1,

            AllowLastWordToGoOverMaxLength = 0x2,

            IncludeEllipsis = 0x4
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

        public static string ResolveCheckBox(bool isChecked)
            => isChecked ? "active" : "inactive";

        public static byte ToByte(object value)
        {
            byte val = 0;

            if (IsEmptyData(value))
            {
                return val;
            }

            byte.TryParse(value.ToString(), out val);

            return val;
        }

        public static byte ToByte(this IDataReader obj, string fieldName)
            => ToByte(obj[fieldName]);

        public static byte ToByte(this DataRow obj, string fieldName)
            => ToByte(obj[fieldName]);

        public static short ToInt16(object value)
        {
            short val = 0;

            if (IsEmptyData(value))
            {
                return val;
            }

            short.TryParse(value.ToString(), out val);

            return val;
        }

        public static short ToInt16(this IDataReader obj, string fieldName)
            => ToInt16(obj[fieldName]);

        public static short ToInt16(this DataRow obj, string fieldName)
            => ToInt16(obj[fieldName]);

        public static decimal ToDecimal(object value)
        {
            if (IsEmptyData(value))
            {
                return 0.0m;
            }

            decimal val;
            decimal.TryParse(value.ToString(), out val);

            return val.Format() == -0.00m ? 0.0m : val;
        }

        public static decimal ToDecimal(this IDataReader obj, string fieldName)
            => ToDecimal(obj[fieldName]);

        public static decimal ToDecimal(this DataRow obj, string fieldName)
            => ToDecimal(obj[fieldName]);

        public static bool ToBoolean(object value)
        {
            if (IsEmptyData(value))
            {
                return false;
            }

            if (value.ToString() == "1")
            {
                return true;
            }

            bool val;

            bool.TryParse(value.ToString(), out val);

            return val;
        }

        public static bool ToBoolean(this IDataReader obj, string fieldName)
        {
            if (IsEmptyData(obj[fieldName]))
            {
                return false;
            }

            bool val;

            bool.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        public static bool ToBoolean(this DataRow obj, string fieldName)
            => ToBoolean(obj[fieldName]);

        public static string ToString(object value)
            => IsEmptyData(value) ? string.Empty : Convert.ToString(value);

        public static string ToString(this IDataReader obj, string fieldName)
            => ToString(obj[fieldName]);

        public static string ToString(this DataRow obj, string fieldName)
            => ToString(obj[fieldName]);

        public static double ToDouble(object value)
        {
            if (IsEmptyData(value))
            {
                return 0.0d;
            }

            double val;

            double.TryParse(value.ToString(), out val);

            return val;
        }

        public static double ToDouble(this IDataReader obj, string fieldName)
            => ToDouble(obj[fieldName]);

        public static double ToDouble(this DataRow obj, string fieldName)
            => ToDouble(obj[fieldName]);

        public static DateTime ToDateTime(object value)
        {
            if (IsEmptyData(value))
            {
                return DateTime.MinValue;
            }

            DateTime val;

            DateTime.TryParse(value.ToString(), out val);

            return val;
        }

        public static DateTime ToDateTime(this IDataReader obj, string fieldName)
            => ToDateTime(obj[fieldName]);

        public static DateTime ToDateTime(this DataRow obj, string fieldName)
            => ToDateTime(obj[fieldName]);

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

        public static long ToInt64(object value)
        {
            if (IsEmptyData(value))
            {
                return 0L;
            }

            long val;

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

        public static long ToInt64(this IDataReader obj, string fieldName)
            => ToInt64(obj[fieldName]);

        public static long ToInt64(this DataRow obj, string fieldName)
            => ToInt64(obj[fieldName]);

        public static decimal Format(object value, int prec = 2)
        {
            var val = ToDecimal(value);

            return decimal.Round(val, prec);
        }

        public static string ToStringFormat(object value, int prec = 2)
            => Format(value, prec).ToString("#,##0.########");

        public static decimal Format(this decimal obj, int prec = 2)
            => decimal.Round(obj, prec);

        public static string ToStringFormat(this decimal obj, int prec = 2)
        {
            var ret = decimal.Round(obj, prec);
            var valRet = ret.ToString("#,##0.########");

            return ret < 0.0m ? "<font color=\"red\">" + valRet + "</font>" : valRet;
        }

        public static decimal Format(this IDataReader obj, string fieldName, int prec = 2)
            => Format(obj[fieldName], prec);

        public static decimal Format(this DataRow obj, string fieldName, int prec = 2)
            => Format(obj[fieldName], prec);

        public static string ToStringFormat(this IDataReader obj, string fieldName, int prec = 2)
            => ToStringFormat(obj[fieldName], prec);

        public static string ToStringFormat(this DataRow obj, string fieldName, int prec = 2)
            => ToStringFormat(obj[fieldName], prec);

        public static string DecodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlDecode(obj.ToString(fieldName));

        public static string EncodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlEncode(obj.ToString(fieldName));

        public static string DecodeHtml(this string str)
            => HttpUtility.HtmlDecode(str);

        public static string EncodeHtml(this string str)
            => HttpUtility.HtmlEncode(str);

        public static string EncodeHtml(this object obj)
            => HttpUtility.HtmlEncode(obj);

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

            var includeEllipsis = (options & TruncateOptions.IncludeEllipsis) == TruncateOptions.IncludeEllipsis;
            var finishWord = (options & TruncateOptions.FinishWord) == TruncateOptions.FinishWord;
            var allowLastWordOverflow = (options & TruncateOptions.AllowLastWordToGoOverMaxLength) == TruncateOptions.AllowLastWordToGoOverMaxLength;

            var retValue = valueToTruncate;

            if (includeEllipsis)
            {
                maxLength -= 1;
            }

            var lastSpaceIndex = retValue.LastIndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);

            if (!finishWord)
            {
                retValue = retValue.Remove(maxLength);
            }
            else if (allowLastWordOverflow)
            {
                var spaceIndex = retValue.IndexOf(" ", maxLength, StringComparison.CurrentCultureIgnoreCase);
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

        public static void EnableLoginBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/login/css",
                "~/bootstrap/login/js",
                Config.CssLogin,
                Config.JsLogin);

        public static void EnableCommonBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/css",
                "~/bootstrap/js",
                Config.CssFiles,
                Config.JsFiles);

        public static void EnableExternalGroupBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/externalgroup/css",
                "~/bootstrap/externalgroup/js",
                Config.CssExtGroup,
                Config.JsExtGroup);

        public static void EnableDailyWinLossBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/dailywinloss/css",
                "~/bootstrap/dailywinloss/js",
                Config.CssDailyWinLoss,
                Config.CssDailyWinLoss);

        public static void EnableWinLossDetailBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/winlossdetail/css",
                "~/bootstrap/winlossdetail/js",
                Config.CssWinLossDetail,
                Config.JsWinLossDetail);

        public static void EnableStatementBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/Statement/css",
                "~/bootstrap/Statement/js",
                Config.CssStatement,
                Config.JsStatement);

        public static void EnableCustomerBundle(this BundleCollection bundles)
            => EnableBundle(
                bundles,
                "~/bootstrap/customer/css",
                "~/bootstrap/customer/js",
                Config.CssCustomer,
                Config.JsCustomer);

        public static string ToStringExt(this DateTime date, string format = "MM/dd/yyyy HH:mm:ss tt")
            => date.Equals(DateTime.MinValue) ? string.Empty : date.ToString(format);

        static bool IsEmptyData(object value) =>
            value == null || value == DBNull.Value || value.ToString() == string.Empty;

        static void EnableBundle(
            this BundleCollection bundles,
            string cssBundle,
            string javascriptBundle,
            string cssConfig,
            string javascriptConfig)
        {
            var bootstrapCss = new Bundle(cssBundle, new CssMinify());
            var bootstrapJs = new Bundle(javascriptBundle, new JsMinify());

            var css = cssConfig.Split(';');
            var javascript = javascriptConfig.Split(';');

            bootstrapCss.Include(css);
            bundles.Add(bootstrapCss);

            bootstrapJs.Include(javascript);
            bundles.Add(bootstrapJs);
        }
    }
}