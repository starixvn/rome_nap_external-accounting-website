namespace Fanex.ProductStatus.Setting.Models
{
    public class ProductProfile
    {
        public ProductProfile()
        {

        }

        public ProductProfile(int productId, byte? settingLevel, byte? settingType = null)
        {
            ProductId = productId;
            SettingLevel = settingLevel;
            SettingType = settingType;
        }

        public int ProductId { get; }

        public int? SettingLevel { get; }

        public int? SettingType { get; }
    }
}
