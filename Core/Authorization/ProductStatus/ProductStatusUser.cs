namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System.Collections.Generic;
    using System.Linq;
    using Context;
    using Fanex.ProductStatus.Setting.Services;

    public class ProductStatusUser : IProductStatusUser
    {
        public int GetActorId()
            => CurrentSession.Instance.UserId;

        public string GetClientIP()
            => CurrentContext.Instance.Ip4Address;

        public IEnumerable<KeyValuePair<int, string>> GetProductList()
        {
            return new ProductListAuthorization()
                .GetProductList()
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId,
                    product.ProductId == 0 ? product.Name : ProductListAuthorization.GetProductNameFromRemotingRef(product.ProductId)))
                .ToList();
        }
    }
}
