namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class RemoveOtpCommand : INonQueryCommand
    {
        public int CustId { get; }

        public int UserId { get; }

        public string Language => "en";

        public string Ip { get; }

        public int ErrCode { get; set; }

        public string ErrMsg { get; set; }

        public RemoveOtpCommand(int custId, int userId, string ip)
        {
            CustId = custId;
            UserId = userId;
            Ip = ip;
        }

        public string GetSettingKey() => "RemoveCustomerOTP";

        public bool IsValid() => CustId > 0 && UserId > 0;
    }
}