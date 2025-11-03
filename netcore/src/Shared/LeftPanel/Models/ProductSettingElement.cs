namespace Fanex.ExternalAccount.Module.LeftPanel.Models
{
    using Core.Shared;

    public enum StatusType
    {
        Checkbox,
        Link,
        OnlyDetail
    }

    public class ProductSettingElement : Product
    {
        public ProductSettingElement(Product product, StatusType statusType)
        {
            Id = product.Id;
            Key = product.Key;
            Name = product.Name;
            StatusType = statusType;
        }

        public StatusType StatusType { get; }
    }
}