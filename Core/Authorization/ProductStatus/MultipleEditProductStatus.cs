namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System.Collections.Generic;
    using System.Linq;
    using Shared;

    public class MultipleEditProductStatus
    {
        private static List<int> UnsupportProductStatus = new List<int>
        {
            ProductConstants.Sportsbook
        };

        protected MultipleEditProductStatus()
        {
        }

        public static IEnumerable<KeyValuePair<int, string>> GetCustomerProductList()
            => new ProductListAuthorization()
                .GetProductList()
                .Where(product => !UnsupportProductStatus.Any(i => i == product.ProductId))
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId, 
                    product.ProductText))
                .ToList();
    }
}
