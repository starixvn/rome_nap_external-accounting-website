namespace Fanex.ExternalAccount.DataAccess.Criteria.UserControl.UserCRUD
{
    using Fanex.Data.Repository;

    public class ResetPasswordCriteria : ResultCriteria, INonQueryCommand
    {
        public int UserID { get; set; }

        public int CustID { get; set; }

        public string UserPwd { get; set; }

        public string IP { get; set; }

        public string GetSettingKey() => "EAS_UserInfoUpdUserPwd";

        public bool IsValid() => true;
    }
}