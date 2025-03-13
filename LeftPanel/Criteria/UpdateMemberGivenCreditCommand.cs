namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using System.Collections;
    using Fanex.Data.Repository;

    public class UpdateMemberGivenCreditCommand : UpdateSMAGivenCreditCommand
    {
        public decimal TotalAmt { get; }

        public UpdateMemberGivenCreditCommand(int id, int uplineId, decimal newGivenCredit, decimal totalAmt, string ip)
            : base(id, uplineId, newGivenCredit, ip)
        {
            TotalAmt = totalAmt;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdCredit";
        }

        public override bool IsValid()
        {
            return base.IsValid() && TotalAmt >= 0;
        }
    }
}