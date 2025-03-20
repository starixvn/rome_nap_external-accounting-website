namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class UpdateUserPolicyCriteria : ResultCriteria, INonQueryCommand
    {
        public UpdateUserPolicyCriteria(int actorID, int custID, string oldUserPolicy, string userPolicy, string ip)
        {
            UserID = actorID;
            CustID = custID;
            OldUserPolicy = oldUserPolicy;
            UserPolicy = userPolicy;
            IP = ip;
        }
        public int UserID { get; set; }
        
        public int CustID { get; set; }
        
        public string OldUserPolicy { get; set; }
        
        public string UserPolicy { get; set; }
        
        public string IP { get; set; }
        
        public string GetSettingKey() => "EAS_UserPolicyUpd";

        public bool IsValid() => UserID > 0 && CustID > 0 && !string.IsNullOrEmpty(OldUserPolicy);
    }
}