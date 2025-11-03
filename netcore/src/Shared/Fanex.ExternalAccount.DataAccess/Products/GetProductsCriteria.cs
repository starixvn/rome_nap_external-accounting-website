namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.Data.Repository;

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