namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetCashAccumCreditCommand : INonQueryCommand
    {
        public int CustId { get; }

        public decimal AccumCreditReturn { get; set; }

        public GetCashAccumCreditCommand(int custId)
        {
            CustId = custId;
        }

        public string GetSettingKey()
        {
            return "Age_CashAccumCredit_AgSel";
        }

        public bool IsValid() => true;
    }
}