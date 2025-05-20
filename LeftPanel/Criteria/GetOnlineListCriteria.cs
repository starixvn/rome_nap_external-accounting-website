namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using Fanex.Data.Repository;

    public class GetOnlineListCriteria : CriteriaBase
    {
        public int CustId { get; }

        public GetOnlineListCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "Acc_CustomerSelOnline";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}