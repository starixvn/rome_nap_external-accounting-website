namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class GetFunctionPolicyCriteria : CriteriaBase
    {
        public int FuncId { get; set; }

        public override string GetSettingKey() => "EAS_FunctionSelEdit";

        public override bool IsValid() => true;
    }
} 