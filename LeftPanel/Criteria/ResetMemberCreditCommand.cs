namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class ResetMemberCreditCommand : INonQueryCommand
    {
        public int UserId { get; }
        public int UplineId { get; }
        public bool ClosedOnly { get; }
        public string Ip { get; }
        public int ErrCode { get; set; }
        public string ErrMsg { get; set; }

        public ResetMemberCreditCommand(int userId, int uplineId, bool closedOnly, string ip)
        {
            UserId = userId;
            UplineId = uplineId;
            ClosedOnly = closedOnly;
            Ip = ip;
        }

        public string GetSettingKey()
        {
            return "Acc_CustomerUpdResetMemberCredit";
        }

        public bool IsValid()
        {
            return UserId > 0 && UplineId > 0;
        }
    }
}