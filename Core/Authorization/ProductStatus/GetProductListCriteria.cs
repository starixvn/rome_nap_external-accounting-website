namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using Fanex.NAP.Framework.Data;

    internal class GetProductListCriteria : CriteriaBase
    {
        internal GetProductListCriteria(int groupId)
        {
            GroupId = groupId;
        }

        public int GroupId { get; }

        public override string GetSettingKey()
            => "EAS_ProductSettingByGroup_Sel";

        public override bool IsValid()
            => GroupId > 0;
    }
}
