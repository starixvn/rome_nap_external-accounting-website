namespace Fanex.ExternalAccount.Utilities
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;

    public static class Config
    {
        public static DateTime WinlostDate => HttpContext.Current.IsDebuggingEnabled ?
            new DateTime(2012, 11, 6).Date :
            DateTime.Now.AddDays(-1).Date;

        public static bool IsTest => Utils.ToBoolean(GetValue("IsTest"));

        public static string LBUrl => GetValue("LBUrl");

        public static string TimeFrom { get; set; }

        public static string TimeTo { get; set; }

        public static bool IsEnabledRecheckin => Utils.ToBoolean(GetValue("Recheckin"));

        public static bool IsShowVirtualSports => Utils.ToBoolean(GetValue("EnableVirtualSports"));

        public static string CssFiles => GetValue("CssFiles");

        public static string JsFiles => GetValue("JsFiles");

        public static string JsLogin => GetValue("JsLogin");

        public static string CssLogin => GetValue("CssLogin");

        public static string JsExtGroup => GetValue("JsExtGroup");

        public static string CssExtGroup => GetValue("CssExtGroup");

        public static string JsDailyWinLoss => GetValue("JsDailyWinLoss");

        public static string CssDailyWinLoss => GetValue("CssDailyWinLoss");

        public static string JsWinLossDetail => GetValue("JsWinLossDetail");

        public static string CssWinLossDetail => GetValue("CssWinLossDetail");

        public static string JsStatement => GetValue("JsStatement");

        public static string CssStatement => GetValue("CssStatement");

        public static string JsCustomer => GetValue("JsCustomer");

        public static string CssCustomer => GetValue("CssCustomer");

        public static string InternalAgentUMToken => GetValue("int_agent_UMToken");

        public static string InternalLoginUrl => GetValue("int_agent_url");

        public static string InternalLoginUrl7 => GetValue("int_agent_url7");

        public static string InternalLoginUrl9 => GetValue("int_agent_url9");

        public static bool HideGoldDeluxe => Utils.ToBoolean(GetValue("HideGoldDeluxe"));

        public static bool IsCache => Utils.ToBoolean(GetValue("IsCache"));

        public static IList<int> AdminUsersEditSuperStatus
        {
            get
            {
                var ids = GetValue("AdminUsersEditSuperStatus").Split(new[] { "|" }, StringSplitOptions.RemoveEmptyEntries);
                IList<int> adminIds = new List<int>();

                foreach (var id in ids)
                {
                    adminIds.Add(Utils.ToInt32(id));
                }

                return adminIds;
            }
        }

        public static int SystemId => Utils.ToInt32(GetValue("SystemId"));

        static string GetValue(string key)
        {
            return ConfigurationManager.AppSettings[key];
        }
    }
}