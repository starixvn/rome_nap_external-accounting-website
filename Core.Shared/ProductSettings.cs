namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class ProductSettings
    {
        public static readonly IReadOnlyDictionary<int, Product> SiteProducts
            = new Dictionary<int, Product>
            {
                [ProductConstants.AllBet] = new Product(ProductConstants.AllBet, "Allbet"),
                [ProductConstants.VoidBridge] = new Product(ProductConstants.VoidBridge, "Macau Games", substituteProductKey: "VoidBridge"),
                [ProductConstants.NG2] = new Product(ProductConstants.NG2, "NG2", true),
                [ProductConstants.FishingWorld] = new Product(ProductConstants.FishingWorld, "Fishing World"),
                [ProductConstants.Lottery] = new Product(ProductConstants.Lottery, "Lottery"),
                [ProductConstants.Lucky3] = new Product(ProductConstants.Lucky3, "Lucky 3"),
                [ProductConstants.BullFighting] = new Product(ProductConstants.BullFighting, "Bullfighting"),
                [ProductConstants.PokerGames] = new Product(ProductConstants.PokerGames, "Poker Games"),
                [ProductConstants.Sportsbook2] = new Product(ProductConstants.Sportsbook2, "Sportsbook 2")
            };

        public static bool IsShowGoldDeluxe => IsShowProduct(ProductConstants.GoldDeluxe);

        public static bool IsShowKeno => IsShowProduct(ProductConstants.Keno);

        public static bool ShowKenoSetting => Configs["ShowKenoSetting"] == "true";

        public static bool IsShowColossusBets => IsShowProduct(ProductConstants.ColossusBets);

        public static bool IsShowCricket => IsShowProduct(ProductConstants.Cricket);

        public static bool IsShowAllbet => IsShowProduct(ProductConstants.AllBet);

        public static bool IsShowVoidBridge => IsShowProduct(ProductConstants.VoidBridge);

        public static bool IsShowNG2 => IsShowProduct(ProductConstants.NG2);

        public static bool IsShowFishingWorld => IsShowProduct(ProductConstants.FishingWorld);

        public static bool IsShowLottery => IsShowProduct(ProductConstants.Lottery);

        public static bool IsShowLucky3 => IsShowProduct(ProductConstants.Lucky3);

        public static bool HideEsports => ToBoolean(Configs["HideEsports"]);

        public static bool IsShowBullfighting => IsShowProduct(ProductConstants.BullFighting);

        public static bool IsShowPokerGames => IsShowProduct(ProductConstants.PokerGames);

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