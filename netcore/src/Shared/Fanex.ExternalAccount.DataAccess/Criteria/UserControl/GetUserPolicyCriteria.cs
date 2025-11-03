namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    public class GetUserPolicyCriteria : GetGroupPolicyCriteria
    {
        public GetUserPolicyCriteria(int funcId)
            : base(funcId)
        {
        }

        public override string GetSettingKey()
            => "EAS_UserPolicySelByFuncID";
    }
}
