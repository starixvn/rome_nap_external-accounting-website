namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetProductsCriteria : CriteriaBase
    {
        public GetProductsCriteria(int? groupId)
        {
            GroupId = groupId;
        }

        public int? GroupId { get; }

        public override string GetSettingKey() => "EAS_GetProductList";

        public override bool IsValid() => true;
    }
}
