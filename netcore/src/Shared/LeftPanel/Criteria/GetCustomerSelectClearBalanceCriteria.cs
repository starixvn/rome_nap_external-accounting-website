namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetCustomerSelectClearBalanceCriteria : CriteriaBase
    {
        public int CustId { get; }

        public GetCustomerSelectClearBalanceCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelClearBalance";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
