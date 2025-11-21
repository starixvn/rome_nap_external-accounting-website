namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Configuration;

    public static class ProductSettings
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static readonly IReadOnlyDictionary<int, Product> SiteProducts
            = new Dictionary<int, Product>
            {
                [ProductConstants.AllBet] = new Product(0, ProductConstants.AllBet, "Allbet"),
                [ProductConstants.SabaVirtualSports] = new Product(9, ProductConstants.SabaVirtualSports, "Saba Virtual Sports"),
                [ProductConstants.Cashout] = new Product(12, ProductConstants.Cashout, "Cashout", true)
            };

        public static bool IsShowProduct(int productId) => ToBoolean(_configuration?[$"IsShowProduct-{productId}"]);

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

