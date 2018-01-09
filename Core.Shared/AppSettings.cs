using System.Collections.Specialized;

namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static bool IsShowGoldDeluxeSettingForCredit => Configs["IsShowGoldDeluxeSettingForCredit"] == "1";

        public static bool EnableKeno => Configs["EnableKeno"] == "true";

        public static bool ShowKenoSetting => Configs["ShowKenoSetting"] == "true";

        public static bool IsShowColossusBets => Configs["IsShowProduct-15"] == "1";

        public static bool IsShowSportsbook2 => Configs["IsShowProduct-16"] == "1";

        public static bool IsShowCockfighting => Configs["IsShowProduct-19"] == "1";

        public static bool IsShowCricket => Configs["IsShowProduct-20"] == "1";

        public static bool IsShowAllbet => Configs["IsShowProduct-21"] == "1";

        public static bool IsShowVoidBridge => Configs["IsShowProduct-22"] == "1";

        public static bool IsShowNG2 => Configs["IsShowProduct-23"] == "1";

        public static bool IsShowFishingWorld => Configs["IsShowProduct-24"] == "1";

        public static bool IsShowLottery => Configs["IsShowProduct-28"] == "1";

        public static bool IsShowLucky3 => Configs["IsShowProduct-26"] == "1";

        public static bool HideEsports => ToBoolean(Configs["HideEsports"]);

        public static bool IsShowBullfighting => ToBoolean(Configs["IsShowProduct-30"]);

        public static bool IsShowProduct(int productId) => ToBoolean(Configs[$"IsShowProduct-{productId}"]);

        private static bool ToBoolean(string value, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value == "1" || value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;
    }
}