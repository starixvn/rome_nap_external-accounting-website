namespace Fanex.ProductStatus.Setting.Criteria
{
    using Fanex.Data.Repository;

    public class SelectProductProfileByGroupCriteria : CriteriaBase
    {
        public SelectProductProfileByGroupCriteria(int groupId, int groupTypeId, int systemId, byte settingStatus, byte mode = 2)
        {
            GroupId = groupId;
            GroupTypeId = groupTypeId;
            SystemId = systemId;
            SettingStatus = settingStatus;
            Mode = mode;
        }

        public int GroupId { get; }

        public int GroupTypeId { get; }

        public int SystemId { get; }

        public byte SettingStatus { get; }

        public byte Mode { get; }

        public override string GetSettingKey() => "Acc_LiteMode_CustSetting_ProductProfileSelect";

        public override bool IsValid() => true;
    }
}
