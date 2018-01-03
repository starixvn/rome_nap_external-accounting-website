namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static bool IsShowGoldDeluxeSettingForCredit => ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "1";

        public static bool EnableKeno => ConfigurationManager.AppSettings["EnableKeno"] == "true";

        public static bool ShowKenoSetting => ConfigurationManager.AppSettings["ShowKenoSetting"] == "true";

        public static bool IsShowColossusBets => ConfigurationManager.AppSettings["IsShowProduct-15"] == "1";

        public static bool IsShowSportsbook2 => ConfigurationManager.AppSettings["IsShowProduct-16"] == "1";

        public static bool IsShowCockfighting => ConfigurationManager.AppSettings["IsShowProduct-19"] == "1";

        public static bool IsShowCricket => ConfigurationManager.AppSettings["IsShowProduct-20"] == "1";

        public static bool IsShowAllbet => ConfigurationManager.AppSettings["IsShowProduct-21"] == "1";

        public static bool IsShowVoidBridge => ConfigurationManager.AppSettings["IsShowProduct-22"] == "1";

        public static bool IsShowNG2 => ConfigurationManager.AppSettings["IsShowProduct-23"] == "1";

        public static bool IsShowFishingWorld => ConfigurationManager.AppSettings["IsShowProduct-24"] == "1";

        public static bool IsShowLottery => ConfigurationManager.AppSettings["IsShowProduct-28"] == "1";

        public static bool IsShowLucky3 => ConfigurationManager.AppSettings["IsShowProduct-26"] == "1";

        public static bool HideEsports => ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);

        public static bool IsShowBullfighting => ToBoolean(ConfigurationManager.AppSettings["IsShowProduct-30"]);

        public static bool IsShowProduct(int productId) => ToBoolean(ConfigurationManager.AppSettings[$"IsShowProduct-{productId}"]);

        private static bool ToBoolean(string value, bool defaultValue = false)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return defaultValue;
            }

            return value == "1" || value.Equals("true", StringComparison.CurrentCultureIgnoreCase);
        }
    }
}