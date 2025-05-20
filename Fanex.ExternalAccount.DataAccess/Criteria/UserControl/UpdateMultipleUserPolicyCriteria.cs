namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    public class UpdateMultipleUserPolicyCriteria : UpdateMultipleGroupPolicyCriteria
    {
        public UpdateMultipleUserPolicyCriteria(
            int funcId,
            string policy,
            string oldPolicy,
            int userId,
            string ip) : base(funcId, policy, oldPolicy, userId, ip)
        {
        }
        
        public override string GetSettingKey() => "EAS_UserPolicyUpdMultiple";
    }
}
