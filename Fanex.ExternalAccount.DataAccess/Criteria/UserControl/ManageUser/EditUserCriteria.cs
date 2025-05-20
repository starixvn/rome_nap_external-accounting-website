namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class EditUserCriteria : ResultCriteria, INonQueryCommand
    {
        public int UserID { get; set; }
        
        public int CustID { get; set; }

        public string FirstName { get; set; }
        
        public string LastName { get; set; }
        
        public int GroupID { get; set; }
        
        public string IP { get; set; }
        
        public string GetSettingKey() => "EAS_UserInfoUpdAll";

        public bool IsValid() => true;
    }
}