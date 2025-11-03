namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class EAS_UserInfoAddNewCriteria : ResultCriteria, INonQueryCommand
    {
        public int UserId { get; set; }

        public string Username { get; set; }

        public string UserPwd { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public int GroupID { get; set; }

        public string IP { get; set; }

        public int CustID { get; set; }

        public string LogData { get; set; }

        public bool IsValid() => UserId > 0;

        public string GetSettingKey() => "EAS_UserInfoAddNew";
    }
}