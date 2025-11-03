namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;

    public class GetGroupPolicyCriteria : CriteriaBase
    {
        public GetGroupPolicyCriteria(int funcId)
        {
            FuncId = funcId;
        }

        public int FuncId { get; }

        public override string GetSettingKey()
            => "EAS_GroupPolicySelByFuncID";

        public override bool IsValid()
            => FuncId > 0;
    }
}