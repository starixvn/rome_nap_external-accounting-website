namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class UpdateStatusCriteria : ResultCriteria, INonQueryCommand
    {
        public UpdateStatusCriteria(int userID, int custID, bool enabled, string ip)
        {
            UserID = userID;
            CustID = custID;
            Enabled = enabled;
            IP = ip;
        }

        public int UserID { get; set; }
        
        public int CustID { get; set; }
        
        public bool Enabled { get; set; }
        
        public string IP { get; set; }
        
        public string GetSettingKey() => "EAS_UserInfoUpdStatus";

        public bool IsValid() => UserID > 0 && CustID > 0;
    }
}