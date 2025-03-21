namespace Fanex.ExternalAccount.DataAccess.Criteria.Login
{
    using Fanex.Data;
    using Fanex.Data.Repository;

    public class CheckinCommand : INonQueryCommand
    {
        public int CustId { get; }
        public string NetSessionId { get; }
        public string IP { get; }
        [SpParam(SqlParamName = "on_userid")]
        public int OnUserId { get; set; }

        public CheckinCommand(int custId, string netSessionId, string ip)
        {
            CustId = custId;
            NetSessionId = netSessionId;
            IP = ip;
        }

        public string GetSettingKey() => "EAS_UserCheckIn";

        public bool IsValid() => CustId > 0 && !string.IsNullOrWhiteSpace(NetSessionId) && !string.IsNullOrWhiteSpace(IP);
    }
}