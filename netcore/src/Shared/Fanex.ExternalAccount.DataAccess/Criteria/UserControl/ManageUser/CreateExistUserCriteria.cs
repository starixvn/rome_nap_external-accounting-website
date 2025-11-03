namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class CreateExistUserCriteria : ResultCriteria, INonQueryCommand
    {
        public CreateExistUserCriteria(int userId, string userName, int groupId, string ip)
        {
            UserID = userId;
            Username = userName;
            GroupID = groupId;
            IP = ip;
        }
        public int UserID { get; set; }
        
        public string Username { get; set; }
        
        public int GroupID { get; set; }
        
        public string IP { get; set; }
        
        public string GetSettingKey() => "EAS_UserInfoAddExistingUser";

        public bool IsValid() => UserID > 0 && !string.IsNullOrEmpty(Username) && GroupID > 0;
    }
}