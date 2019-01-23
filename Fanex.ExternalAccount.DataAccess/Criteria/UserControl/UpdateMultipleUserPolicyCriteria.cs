namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    public class UpdateMultipleUserPolicyCriteria : UpdateMultipleGroupPolicyCriteria
    {
        public UpdateMultipleUserPolicyCriteria(
            int funcId,
            string policy,
            int userId,
            string ip) : base(funcId, policy, userId, ip)
        {
        }

        public override string GetSettingKey()
            => "EAS_UserPolicyUpdMultiple";
    }
}
