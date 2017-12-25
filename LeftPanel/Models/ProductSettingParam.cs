namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    using RemotingRef;

    public enum StatusType
    {
        Checkbox,
        Link
    }

    public class ProductSettingParam
    {
        public ProductSettingParam(int productId, string productName, StatusType statusType, bool isGetNameFromRemotingRef)
        {
            ProductKey = string.IsNullOrWhiteSpace(productName)
                          ? string.Empty
                          : productName.Replace(" ", string.Empty);
            ProductId = productId;
            ProductName = isGetNameFromRemotingRef
                        ? RefData.GetResourceDataByRefIdAndResourceId("Product", productId.ToString(), "en-US", productName)
                        : productName;
            StatusType = statusType;
        }

        public int ProductId { get; }

        public string ProductName { get; }

        public StatusType StatusType { get; }

        public string ProductKey { get; }
    }
}