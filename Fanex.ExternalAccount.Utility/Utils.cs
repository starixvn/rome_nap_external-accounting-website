namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Text;
    using System.Web.Optimization;

    public static class Utils
    {
        public enum TruncateOptions
        {
            None = 0x0,
            FinishWord = 0x1,
            AllowLastWordToGoOverMaxLength = 0x2,
            IncludeEllipsis = 0x4
        }

        public static string ResolveCheckBox(bool isChecked)
        {
            return isChecked ? "active" : "inactive";
        }

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

        public static bool ToBoolean(this DataRow obj, string fieldName)
        {
            var val = false;

            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return false;
            }

            bool.TryParse(obj[fieldName].ToString(), out val);

            return val;
        }

        public static string ToString(object value)
        {
            if (value == null || value == DBNull.Value || value.ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(value);
        }

        public static string ToString(this IDataReader obj, string fieldName)
        {
            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(obj[fieldName]);
        }

        public static string ToString(this DataRow obj, string fieldName)
        {
            if (obj[fieldName] == null || obj[fieldName] == DBNull.Value || obj[fieldName].ToString() == string.Empty)
            {
                return string.Empty;
            }

            return Convert.ToString(obj[fieldName]);
        }

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

        /// <summary>
        /// Truncates a string to a certain length
        /// </summary>
        /// <param name="valueToTruncate"></param>
        /// <param name="maxLength">Maximum length that the string should allow</param>
        /// <param name="options">Option flags to determine how to truncate the string
        ///		<para>&#160;&#160;&#160;&#160;<c>None</c>: Just cut off the word at the maximum length</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>FinishWord</c>: Make sure that the string is not truncated in the middle of a word</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>AllowLastWordToGoOverMaxLength</c>: If FinishWord is set, this allows the string to be longer than the maximum length if there is a word started and not finished before the maximum length</para>
        ///		<para>&#160;&#160;&#160;&#160;<c>IncludeEllipsis</c>: Include an ellipsis HTML character at the end of the truncated string.  This counts as one of the characters for the maximum length</para>
        /// </param>
        /// <returns>Truncated string</returns>
        public static string TruncateString(string valueToTruncate, int maxLength, TruncateOptions options)
        {
            if (valueToTruncate == null || maxLength <= 0)
            {
                return "";
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

        public static void EnableLoginBundle(this BundleCollection bundles)
        {
            // For css and js login using
            var bootstrapCssLogin = new Bundle("~/bootstrap/login/css", new CssMinify());
            var bootstrapJsLogin = new Bundle("~/bootstrap/login/js", new JsMinify());

            var cssLogin = Config.CssLogin.Split(';');
            var jsLogin = Config.JsLogin.Split(';');

            foreach (var item in cssLogin)
            {
                bootstrapCssLogin.AddFile(item);
            }

            bundles.Add(bootstrapCssLogin);

            foreach (var item in jsLogin)
            {
                bootstrapJsLogin.AddFile(item);
            }

            bundles.Add(bootstrapJsLogin);
        }

        public static void EnableCommonBundle(this BundleCollection bundles)
        {
            // For css and js common using
            var bootstrapCss = new Bundle("~/bootstrap/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/js", new JsMinify());

            var css = Config.CssFiles.Split(';');
            var js = Config.JsFiles.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }

        public static void EnableExternalGroupBundle(this BundleCollection bundles)
        {
            // For css and js external group selection using
            var bootstrapCss = new Bundle("~/bootstrap/externalgroup/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/externalgroup/js", new JsMinify());

            var css = Config.CssExtGroup.Split(';');
            var js = Config.JsExtGroup.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }

        public static void EnableDailyWinLossBundle(this BundleCollection bundles)
        {
            // For css and js report using
            var bootstrapCss = new Bundle("~/bootstrap/dailywinloss/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/dailywinloss/js", new JsMinify());

            var css = Config.CssDailyWinLoss.Split(';');
            var js = Config.JsDailyWinLoss.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }

        public static void EnableWinLossDetailBundle(this BundleCollection bundles)
        {
            // For css and js report using
            var bootstrapCss = new Bundle("~/bootstrap/winlossdetail/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/winlossdetail/js", new JsMinify());

            var css = Config.CssWinLossDetail.Split(';');
            var js = Config.JsWinLossDetail.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }

        public static void EnableStatementBundle(this BundleCollection bundles)
        {
            // For css and js external group selection using
            var bootstrapCss = new Bundle("~/bootstrap/Statement/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/Statement/js", new JsMinify());

            var css = Config.CssStatement.Split(';');
            var js = Config.JsStatement.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }

        public static void EnableCustomerBundle(this BundleCollection bundles)
        {
            // For css and js external group selection using
            var bootstrapCss = new Bundle("~/bootstrap/customer/css", new CssMinify());
            var bootstrapJs = new Bundle("~/bootstrap/customer/js", new JsMinify());

            var css = Config.CssCustomer.Split(';');
            var js = Config.JsCustomer.Split(';');

            foreach (var item in css)
            {
                bootstrapCss.AddFile(item);
            }

            bundles.Add(bootstrapCss);

            foreach (var item in js)
            {
                bootstrapJs.AddFile(item);
            }

            bundles.Add(bootstrapJs);
        }
    }
}