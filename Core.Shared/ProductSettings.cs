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
                [ProductConstants.AllBet] = new Product(0, ProductConstants.AllBet, "Allbet"),
                [ProductConstants.LiveCasino] = new Product(1, ProductConstants.LiveCasino, "Live Casino"),
                [ProductConstants.VoidBridge] = new Product(2,ProductConstants.VoidBridge, "Macau Games", substituteProductKey: "VoidBridge"),
                [ProductConstants.NG2] = new Product(3, ProductConstants.NG2, "NG2", true),
                [ProductConstants.FishingWorld] = new Product(4, ProductConstants.FishingWorld, "Fishing World"),
                [ProductConstants.Sportsbook2] = new Product(5, ProductConstants.Sportsbook2, "Sportsbook 2"),
                [ProductConstants.VirtualGames] = new Product(6, ProductConstants.VirtualGames, "Virtual Games"),
                [ProductConstants.Sabaclub] = new Product(7, ProductConstants.Sabaclub, "Saba.club"),
                [ProductConstants.RNGLottery] = new Product(8, ProductConstants.RNGLottery, "RNG Lottery"),
            };

        public static bool IsShowGoldDeluxe => IsShowProduct(ProductConstants.GoldDeluxe);

        public static bool IsShowKeno => IsShowProduct(ProductConstants.Keno);

        public static bool ShowKenoSetting => Configs["ShowKenoSetting"] == "true";

        public static bool IsShowLiveCasino => IsShowProduct(ProductConstants.LiveCasino);

        public static bool IsShowVoidBridge => IsShowProduct(ProductConstants.VoidBridge);

        public static bool IsShowNG2 => IsShowProduct(ProductConstants.NG2);

        public static bool IsShowFishingWorld => IsShowProduct(ProductConstants.FishingWorld);

        public static bool IsShowRNGLottery => IsShowProduct(ProductConstants.RNGLottery);

        public static bool HideEsports => ToBoolean(Configs["HideEsports"]);

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