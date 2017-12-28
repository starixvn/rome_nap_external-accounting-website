namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Configuration;

    public static class AppSettings
    {
        public static bool IsShowGoldDeluxeSettingForCredit => ConfigurationManager.AppSettings["IsShowGoldDeluxeSettingForCredit"] == "1";

        public static bool EnableKeno => ConfigurationManager.AppSettings["EnableKeno"] == "true";

        public static bool ShowKenoSetting => ConfigurationManager.AppSettings["ShowKenoSetting"] == "true";

        public static bool IsShowColossusBets => ConfigurationManager.AppSettings["IsShowColossusBets"] == "1";

        public static bool IsShowSportsbook2 => ConfigurationManager.AppSettings["IsShowSportsbook2"] == "1";

        public static bool IsShowCockfighting => ConfigurationManager.AppSettings["IsShowCockfighting"] == "1";

        public static bool IsShowCricket => ConfigurationManager.AppSettings["IsShowCricket"] == "1";

        public static bool IsShowAllbet => ConfigurationManager.AppSettings["IsShowAllbet"] == "1";

        public static bool IsShowVoidBridge => ConfigurationManager.AppSettings["IsShowVoidBridge"] == "1";

        public static bool IsShowNG2 => ConfigurationManager.AppSettings["IsShowNG2"] == "1";

        public static bool IsShowFishingWorld => ConfigurationManager.AppSettings["IsShowFishingWorld"] == "1";

        public static bool IsShowLottery => ConfigurationManager.AppSettings["IsShowLottery"] == "1";

        public static bool IsShowLucky3 => ConfigurationManager.AppSettings["IsShowLucky3"] == "1";

        public static bool HideEsports => ToBoolean(ConfigurationManager.AppSettings["HideEsports"]);

        public static bool IsShowBullfighting => ToBoolean(ConfigurationManager.AppSettings["IsShowBullfighting"]);

        public static bool IsShowProduct(string productName)
        {
            if (productName.Equals("MacauGames", StringComparison.CurrentCultureIgnoreCase))
            {
                productName = "VoidBridge";
            }

            return ToBoolean(ConfigurationManager.AppSettings["IsShow" + productName.Replace(" ", string.Empty)]);
        }

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