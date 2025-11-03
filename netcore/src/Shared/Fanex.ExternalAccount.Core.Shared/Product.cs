namespace Fanex.ExternalAccount.Core.Shared
{

    public class Product
    {
        protected Product()
        {
        }

        public Product(
            int index,
            int productId,
            string productName,
            bool isGetNameFromRemotingRef = false,
            string substituteProductKey = null)
        {
            Index = index;
            Id = productId;
            Key = string.IsNullOrWhiteSpace(productName)
                ? string.Empty
#pragma warning disable S3358 // Ternary operators should not be nested
                : string.IsNullOrWhiteSpace(substituteProductKey) 
                    ? productName.Replace(" ", string.Empty) 
                    : substituteProductKey;
#pragma warning restore S3358 // Ternary operators should not be nested
            Name = isGetNameFromRemotingRef
                ? CacheRef.GetProductName(productId, productName)
                : productName;
        }

        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public string Key { get; protected set; }

        public int Index { get; protected set; }

        public bool IsShow => ProductSettings.IsShowProduct(Id);
    }
}

