namespace Fanex.ExternalAccount.DataAccess.Criteria.SubGroup
{
    using Fanex.Data.Repository;

    public class UpdateSubGroupAdminPwdCriteria : INonQueryCommand
    {
        public int CustID { get; set; }

        public int UserID { get; set; }

        public string UserPwd { get; set; }

        public string IP { get; set; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public string GetSettingKey()
            => "EAS_UserInfoUpdSubGroupAdminPwd";

        public bool IsValid()
            => CustID > 0 && UserID > 0 && !string.IsNullOrWhiteSpace(UserPwd);
    }
}