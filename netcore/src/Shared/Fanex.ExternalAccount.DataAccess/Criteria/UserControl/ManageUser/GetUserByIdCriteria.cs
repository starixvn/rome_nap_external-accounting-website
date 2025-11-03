namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class GetUserByIdCriteria : ICriteria
    { 
        public GetUserByIdCriteria(int userId) => UserID = userId;
        
        public int UserID { get; set; }

        public string GetSettingKey() => "EAS_UserInfoSelByUserID";

        public bool IsValid() => true;
    }
}