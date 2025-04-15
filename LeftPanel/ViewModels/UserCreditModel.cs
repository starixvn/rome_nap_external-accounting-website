namespace Fanex.ExternalAccount.Module.LeftPanel.ViewModels
{
    public class UserCreditModel
    {
        public MasterAgentUsedCreditModel MasterAgentUsedCredit { get; set; }
        
        public MemberUsedCreditModel  DirectMemberUsedCredit { get; set; }
        
        public int RoleId { get; set; }
    }
}