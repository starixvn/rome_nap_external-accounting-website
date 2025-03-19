namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class GetAllFunctionsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EAS_FunctionSel";

        public override bool IsValid() => true;
    }
}