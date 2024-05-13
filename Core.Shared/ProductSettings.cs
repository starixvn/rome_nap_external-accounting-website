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
                [ProductConstants.RNGLottery] = new Product(8, ProductConstants.RNGLottery, "RNG Lottery"),
                [ProductConstants.SabaVirtualSports] = new Product(9, ProductConstants.SabaVirtualSports, "Saba Virtual Sports"),
                [ProductConstants.Bitcoin] = new Product(11, ProductConstants.Bitcoin, "Bitcoin"),
                [ProductConstants.Cashout] = new Product(12, ProductConstants.Cashout, "Cashout", true),
            };

        public static bool IsShowKeno => IsShowProduct(ProductConstants.Keno);

        public static bool IsShowRNGLottery => IsShowProduct(ProductConstants.RNGLottery);

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