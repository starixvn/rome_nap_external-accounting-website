namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class LogoutCommand : INonQueryCommand
    {
        [SpParam(SqlParamName = "on_userid")]
        public int CustId { get; }
        public string Username { get; }

        public LogoutCommand(int custId, string username)
        {
            CustId = custId;
            Username = username;
        }

        public string GetSettingKey() => "EAS_UserLogout";

        public bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(Username);
    }
}

