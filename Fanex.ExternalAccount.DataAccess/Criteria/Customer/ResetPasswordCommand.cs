namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class ResetPasswordCommand : INonQueryCommand
    {
        public ResetPasswordCommand(int custId, string password, int userId, string ip)
        {
            CustId = custId;
            Password = Common.Cryptography.CfsEnCode(password);
            UserId = userId;
            Language = "en";
            SubAccId = 0;
            DisCasino = 0;
            LoginIp = ip;
        }

        public int CustId { get; }
        [SpParam(SqlParamName = "userpwd")]
        public string Password { get; }
        public int UserId { get; }
        public int DisCasino { get; }
        public string Language { get; }
        public int SubAccId { get; }
        public string LoginIp { get; }
        public int ErrCode { get; }
        public string ErrMsg { get; }

        public string GetSettingKey() => "Age_CustomerUpdPwd";

        public bool IsValid() => true;
    }
}
