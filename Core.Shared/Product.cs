namespace Fanex.ExternalAccount.Core.Shared
{
    public class Product
    {
        protected Product()
        {
        }

        public Product(
            int productId,
            string productName,
            bool isGetNameFromRemotingRef = false,
            string substituteProductKey = null)
        {
            Id = productId;
            Key = string.IsNullOrWhiteSpace(productName)
                ? string.Empty
                : string.IsNullOrWhiteSpace(substituteProductKey) 
                    ? productName.Replace(" ", string.Empty) 
                    : substituteProductKey;
            Name = isGetNameFromRemotingRef
                ? RemotingRef.RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                : productName;
        }

        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public string Key { get; protected set; }

        public bool IsShow => ProductSettings.IsShowProduct(Id);
    }
}