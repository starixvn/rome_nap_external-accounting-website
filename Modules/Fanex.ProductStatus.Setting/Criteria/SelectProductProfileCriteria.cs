namespace Fanex.ProductStatus.Setting.Criteria
{
    using Fanex.Data.Repository;

    public class SelectProductProfileCriteria : CriteriaBase
    {
        public SelectProductProfileCriteria(int productId, int settingType, int systemId, byte settingStatus)
        {
            ProductId = productId;
            SettingType = settingType;
            SystemId = systemId;
            SettingStatus = settingStatus;
        }

        public int ProductId { get; }

        public int SettingType { get; }

        public int SystemId { get; }

        public byte SettingStatus { get; }

        public override string GetSettingKey() => "Acc_CustSetting_ProductProfileSelect";

        public override bool IsValid() => ProductId > 0 && SystemId > 0 && (SettingType == 13 || SettingType == 11);
    }
}
