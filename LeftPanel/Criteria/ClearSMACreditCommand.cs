namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class ClearSMACreditCommand : INonQueryCommand
    {
        public ClearSMACreditCommand(string username, int userId, string ip)
        {
            Username = username;
            UserId = userId;
            Ip = ip;
            System = "EXTERNAL";
            ReturnErrCode = 1;
            ErrCode = 0;
            ErrMsg = string.Empty;
        }

        public string Username { get; }
        public int UserId { get; }
        public string Ip { get; }
        public string System { get; }
        public int ReturnErrCode { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public string GetSettingKey() => "Acc_CustomerUpdClearCredit";

        public bool IsValid() => true;
    }
}
