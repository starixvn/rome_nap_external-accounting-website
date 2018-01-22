namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class ProductSettings
    {
        public static bool IsShowGoldDeluxe => IsShowProduct(ProductConstants.GoldDeluxe);

        public static bool IsShowKeno => IsShowProduct(ProductConstants.Keno);

        public static bool ShowKenoSetting => Configs["ShowKenoSetting"] == "true";

        public static bool IsShowColossusBets => IsShowProduct(ProductConstants.ColossusBets);

        public static bool IsShowSportsbook2 => IsShowProduct(ProductConstants.Sportsbook2);

        public static bool IsShowCockfighting => IsShowProduct(ProductConstants.CockFighting);

        public static bool IsShowCricket => IsShowProduct(ProductConstants.Cricket);

        public static bool IsShowAllbet => IsShowProduct(ProductConstants.AllBet);

        public static bool IsShowVoidBridge => IsShowProduct(ProductConstants.VoidBridge);

        public static bool IsShowNG2 => IsShowProduct(ProductConstants.NG2);

        public static bool IsShowFishingWorld => IsShowProduct(ProductConstants.FishingWorld);

        public static bool IsShowLottery => IsShowProduct(ProductConstants.Lottery);

        public static bool IsShowLucky3 => IsShowProduct(ProductConstants.Lucky3);

        public static bool HideEsports => ToBoolean(Configs["HideEsports"]);

        public static bool IsShowBullfighting => IsShowProduct(ProductConstants.BullFighting);

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static bool IsShowProduct(int productId) => ToBoolean(Configs[$"IsShowProduct-{productId}"]);

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