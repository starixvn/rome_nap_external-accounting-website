using Fanex.ExternalAccount.Core.Shared;

namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetProductsCriteria : CriteriaBase
    {
        public GetProductsCriteria(string groupName, string products)
        {
            GroupName = groupName;
            Products = products;
        }

        public string Products { get; }

        public string GroupName { get; }
        public override string GetSettingKey() => "EAS_GetProductList";

        public override bool IsValid() => true;
    }
}
