namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data;

    public class UpdateMemberGivenCreditCommand : UpdateSMAGivenCreditCommand
    {
        [SpParam(SqlParamName = "totalamt")]
        public decimal TotalAmt { get; }

        [SpParam(SqlParamName = "accumcreditallcashmember")]
        public decimal AccumCreditAllCashMember { get; }

        public UpdateMemberGivenCreditCommand(int id, int userId, decimal newGivenCredit, decimal totalAmt, decimal accumCreditAllCashMember, string ip)
            : base(id, userId, newGivenCredit, ip)
        {
            TotalAmt = totalAmt;
            AccumCreditAllCashMember = accumCreditAllCashMember;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerUpdCredit";
        }
    }
}