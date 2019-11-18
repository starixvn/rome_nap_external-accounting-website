using Fanex.RemotingRef;

namespace Fanex.ExternalAccount.Core.Shared
{
    
    public class Product
    {
        private readonly IRemotingRefService _remotingRefService;

        protected Product(): this(null)
        {
        }

        protected Product(IRemotingRefService remotingRefService)
        {
            _remotingRefService = remotingRefService ?? new RemotingRefService();
        }

        public Product(
            int productId,
            string productName,
            bool isGetNameFromRemotingRef = false,
            string substituteProductKey = null):this(new RemotingRefService())
        {
            Id = productId;
            Key = string.IsNullOrWhiteSpace(productName)
                ? string.Empty
#pragma warning disable S3358 // Ternary operators should not be nested
                : string.IsNullOrWhiteSpace(substituteProductKey) 
                    ? productName.Replace(" ", string.Empty) 
                    : substituteProductKey;
#pragma warning restore S3358 // Ternary operators should not be nested
            Name = isGetNameFromRemotingRef
                ? _remotingRefService.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                : productName;
        }

        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public string Key { get; protected set; }

        public bool IsShow => ProductSettings.IsShowProduct(Id);
    }
}