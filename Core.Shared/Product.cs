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
            bool isGetNameFromRemotingRef = false)
        {
            Id = productId;
            Key = string.IsNullOrWhiteSpace(productName)
                ? string.Empty
                : productName.Replace(" ", string.Empty);
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