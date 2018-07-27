namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Common;
    using Fanex.NAP.Framework.Data;

    public class ResetPasswordCriteria : CriteriaBase
    {
        public ResetPasswordCriteria(int custId, string password, int userId, string ip)
        {
            CustId = custId;
            Password = Cryptography.CfsEnCode(password);
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

        public override string GetSettingKey() => "Age_CustomerUpdPwd";
    }
}
