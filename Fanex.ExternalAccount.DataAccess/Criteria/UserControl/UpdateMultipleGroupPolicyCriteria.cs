namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl
{
    using Fanex.Data.Repository;
    
    public class UpdateMultipleGroupPolicyCriteria : CriteriaBase
    {
        public UpdateMultipleGroupPolicyCriteria(
            int funcId,
            string policy,
            string oldPolicy,
            int userId,
            string ip)
        {
            FuncId = funcId;
            Policy = policy;
            OldPolicy = oldPolicy;
            UserId = userId;
            IP = ip;
        }

        public int FuncId { get; }

        public string Policy { get; }

        public string OldPolicy { get; }

        public int UserId { get; }

        public string IP { get; }
        
        public int ErrCode { get; set; }
        
        public string ErrMsg { get; set; }

        public override string GetSettingKey()
            => "EAS_GroupPolicyUpdMultiple";

        public override bool IsValid()
            => FuncId > 0
            && !string.IsNullOrWhiteSpace(Policy)
            && UserId > 0;
    }
}
