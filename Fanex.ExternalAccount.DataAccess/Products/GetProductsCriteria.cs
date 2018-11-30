namespace Fanex.ExternalAccount.DataAccess.Products
{
    using Fanex.NAP.Framework.Data;

    public class GetProductsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_Rpt_GetProductList";

        public override bool IsValid() => true;
    }
}
