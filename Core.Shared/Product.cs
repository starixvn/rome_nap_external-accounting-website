namespace Fanex.ExternalAccount.Core.Shared
{
    using System;

    public class Product
    {
        protected Product() { }

        public Product(
            int productId,
            string productName,
            bool isGetNameFromRemotingRef = false,
            Func<int, bool> checkShowProduct = null)
        {
            Id = productId;
            Key = string.IsNullOrWhiteSpace(productName)
                ? string.Empty
                : productName.Replace(" ", string.Empty);
            Name = isGetNameFromRemotingRef
                ? RemotingRef.RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                : productName;

            IsShow = checkShowProduct?.Invoke(Id) ?? ProductSettings.IsShowProduct(Id);
        }



        public int Id { get; protected set; }

        public string Name { get; protected set; }

        public string Key { get; protected set; }

        public bool IsShow { get; }
    }
}