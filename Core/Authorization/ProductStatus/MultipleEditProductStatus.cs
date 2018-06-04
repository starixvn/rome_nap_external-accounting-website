namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System.Collections.Generic;
    using System.Linq;

    public static class MultipleEditProductStatus
    {
        public static IEnumerable<KeyValuePair<int, string>> GetCustomerProductList()
            => new ProductListAuthorization()
                .GetProductList(true)
                .Where(product => product.ProductId > 1)
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId,
                    product.ProductId == 0 ? product.Name : ProductListAuthorization.GetProductNameFromRemotingRef(product.ProductId)))
                .ToList();
    }
}
