namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Utilities;

    internal class GetProductListCriteria : CriteriaBase
    {
        internal GetProductListCriteria(int groupId)
        {
            GroupId = groupId;
        }

        public int GroupId { get; }

        public byte SettingStatus => Config.SettingStatus;

        public override string GetSettingKey()
            => "EAS_ProductSettingByGroup_Sel";

        public override bool IsValid()
            => GroupId > 0;
    }
}
