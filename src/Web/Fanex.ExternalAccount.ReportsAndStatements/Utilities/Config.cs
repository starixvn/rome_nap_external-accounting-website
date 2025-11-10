using Microsoft.Extensions.Configuration;
using System;
using System.Linq;

namespace Fanex.ExternalAccount.ReportsAndStatements.Utilities
{
    public static class Config
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static DateTime WinlostDate
            => DateTime.Now.AddDays(-1).Date;

        public static bool IsTest => GetValue("IsTest").ToLower() == "true";

        public static string LbUrl => GetValue("LBUrl");

        public static string TimeFrom { get; set; }

        public static string TimeTo { get; set; }

        public static bool IsEnabledRecheckin => GetValue("Recheckin").ToLower() == "true";

        public static bool IsShowVirtualSports => GetValue("EnableVirtualSports").ToLower() == "true";

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

        public static string InternalLoginUrl8 => GetValue("int_agent_url8");

        public static bool IsCache => GetValue("IsCache").ToLower() == "true";

        public static int SystemId => int.TryParse(GetValue("SystemId"), out var result) ? result : 0;

        public static byte SettingStatus => byte.TryParse(GetValue("SettingStatus"), out var result) ? result : (byte)0;

        public static string[] PublicDomains => GetValue("PublicDomains").Split(',');

        public static bool IsRestrictAccessPublicDomain => GetValue("IsRestrictAccessPublicDomain").ToLower() == "true";

        public static string IgnoreRecheckinCustomers => GetValue("IgnoreRecheckinCustomers");

        public static string[] GroupsCanViewWinLossDetailForIBCBetLiveCasino()
        {
            return GetValue("GroupsCanViewWinLossDetailForIBCBetLiveCasino")
                .Split(',')
                .Select(group => group.Trim())
                .ToArray();
        }

        public static string[] GroupsCanViewExtraPT()
        {
            return GetValue("GroupsCanViewExtraPT")
                .Split(',')
                .Select(group => group.Trim())
                .ToArray();
        }

        private static string GetValue(string key)
        {
            return _configuration?[key] ?? string.Empty;
        }
    }
}
