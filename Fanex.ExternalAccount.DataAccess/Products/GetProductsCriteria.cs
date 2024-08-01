using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetProductsCriteria : CriteriaBase
    {
        public GetProductsCriteria(int? groupId, string products)
        {
            GroupId = groupId;
            Products = products;
        }

        public int? GroupId { get; }

        public string Products { get; }

        public override string GetSettingKey() => "EAS_GetProductList";

        public override bool IsValid() => true;
    }
}
