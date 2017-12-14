namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Data;
    using System.Web;
    using System.Web.Optimization;

    public static class Utils
    {
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

            decimal val;
            decimal.TryParse(value.ToString(), out val);

            return val.Format() == -0.00m ? 0.0m : val;
        }

        public static decimal ToDecimal(this IDataReader obj, string fieldName)
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

        public static decimal Format(this decimal obj, int prec = 2)
            => decimal.Round(obj, prec);

        public static string DecodeHtml(this string str)
            => HttpUtility.HtmlDecode(str);

        public static string EncodeHtml(this string str)
            => HttpUtility.HtmlEncode(str);

        public static string DecodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlDecode(obj.ToString(fieldName));

        public static string EncodeHtml(this DataRow obj, string fieldName)
            => HttpUtility.HtmlEncode(obj.ToString(fieldName));

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
                Config.JsDailyWinLoss);

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

        public static string ToStringExt(this DateTime date, string format = "MM/dd/yyyy HH:mm:ss tt")
            => date.Equals(DateTime.MinValue) ? string.Empty : date.ToString(format);

        private static bool IsEmptyData(object value) =>
            value == null || value == DBNull.Value || value.ToString()?.Length == 0;

        private static void EnableBundle(
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