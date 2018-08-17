namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using RemotingRef;
    using System.Collections.Generic;
    using System.Linq;

    public class MultipleEditProductStatus
    {
        private readonly IRemotingRefService _remotingRefService;

        public MultipleEditProductStatus():this(null)
        { }

        public MultipleEditProductStatus(IRemotingRefService remotingRefService)
        {
            _remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public IEnumerable<KeyValuePair<int, string>> GetCustomerProductList()
            => new ProductListAuthorization()
                .GetProductList(true)
                .Where(product => product.ProductId > 1)
                .Select(product => new KeyValuePair<int, string>(
                    product.ProductId,
                    product.ProductId == 0 ? product.Name : _remotingRefService.GetResourceDataByRefIdAndResourceId("Product", product.ProductId.ToString(), "en-US", null)))
                .ToList();
    }
}
