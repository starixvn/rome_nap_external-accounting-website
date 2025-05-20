using Fanex.Data.Repository;

namespace Fanex.ProductStatus.Setting.Criteria
{
    public class GetProductStatusByGroupIdCriteria : CriteriaBase
    {
        public GetProductStatusByGroupIdCriteria(int custId, int groupId, byte settingStatus, int systemId)
        {
            CustId = custId;
            GroupId = groupId;
            SettingStatus = settingStatus;
            SystemId = systemId;
        }

        public int CustId { get; }

        public int GroupId { get; }

        public byte SettingStatus { get; }

        public int SystemId { get; }

        public override string GetSettingKey() => "Acc_Admin_DisableNewProductStatus_160_Select";

        public override bool IsValid() => true;
    }
}