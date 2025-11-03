namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class ResetSecurityCodeCommand : INonQueryCommand
    {
        public int CustId { get; }
        public int UserId { get; }
        public bool IsForcedChangeSecurityCode { get; }
        public string SecurityCode { get; }
        public string Language { get; }
        public string Ip { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public ResetSecurityCodeCommand(int custId, int userId, bool isForcedChangeSecurityCode, string securityCode, string language, string ip)
        {
            CustId = custId;
            UserId = userId;
            IsForcedChangeSecurityCode = isForcedChangeSecurityCode;
            SecurityCode = securityCode;
            Language = language;
            Ip = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdSecurityCode";
        }

        public bool IsValid()
        {
            return CustId > 0 && !string.IsNullOrWhiteSpace(SecurityCode);
        }
    }
}