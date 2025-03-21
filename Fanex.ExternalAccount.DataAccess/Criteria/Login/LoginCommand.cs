namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class LoginCommand : INonQueryCommand
    {
        public string Username { get; }
        [SpParam(SqlParamName = "userpwd")]
        public string Password { get; }
        [SpParam(SqlParamName ="BrowserName")]
        public string Browser { get; }
        [SpParam(SqlParamName = "LoginIP")]
        public string IP { get; }
        [SpParam(SqlParamName = "LoginDomain")]
        public string Domain { get; }
        public string CountryCode { get; }
        public string Platform { get; }
        public int CustId { get; set; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }
        public int GroupID { get; set; }

        public LoginCommand(string username, string password, string browser, string ip, string domain, string countryCode, string platform)
        {
            Username = username;
            Password = Common.Cryptography.CfsEnCode(password);
            Browser = browser;
            IP = ip;
            Domain = domain;
            CountryCode = countryCode;
            Platform = platform;
        }

        public string GetSettingKey() => "EAS_UserLogin";

        public bool IsValid() => !string.IsNullOrEmpty(Username) && !string.IsNullOrEmpty(Password);
    }
}
