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

        public int ErrCode { get; set; }
        
        public string ErrMsg { get; set; }
        
        public override string GetSettingKey()
            => "EAS_UserPolicyUpdMultiple";
    }
}
