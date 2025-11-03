namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class ClearMemberCreditCommand : ClearSMACreditCommand
    {

        public ClearMemberCreditCommand(decimal totalAmt, string username, int userId, string ip)
            : base(username, userId, ip)
        {
            TotalAmt = totalAmt;
        }

        public decimal TotalAmt { get; }

        public string GetSettingKey() => "Acc_CustomerUpdClearCredit";

        public bool IsValid() => TotalAmt > 0;
    }
}