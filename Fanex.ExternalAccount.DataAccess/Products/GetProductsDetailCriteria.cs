namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.Data.Repository;

    public class GetProductsDetailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_Rpt_GetProductDetail";

        public override bool IsValid() => true;
    }
}