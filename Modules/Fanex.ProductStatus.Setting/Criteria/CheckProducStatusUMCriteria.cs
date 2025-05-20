namespace Fanex.ProductStatus.Setting.Criteria
{
    using Fanex.Data.Repository;

    internal class CheckProducStatusUMCriteria : CriteriaBase
    {
        public CheckProducStatusUMCriteria(int custId, int productId = 0, int groupId = 0, byte settingStatus = 0)
            : this(custId.ToString(), productId, groupId, settingStatus)
        {
        }

        public CheckProducStatusUMCriteria(string custIds, int productId = 0, int groupId = 0, byte settingStatus = 0)
        {
            ProductId = productId;
            CustIds = custIds;
            GroupId = groupId;
            SettingStatus = settingStatus;
        }

        public int ProductId { get; }

        public string CustIds { get; }

        public int GroupId { get; }

        public byte SettingStatus { get; }

        public override string GetSettingKey()
            => "Acc_CheckUM_MultipleCustProductStatus";

        public override bool IsValid()
            => ProductId > 0
            && !string.IsNullOrWhiteSpace(CustIds);
    }
}