namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetProductsDetailCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_Rpt_GetProductDetail";

        public override bool IsValid() => true;
    }
}
