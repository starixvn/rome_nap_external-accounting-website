namespace Fanex.ExternalAccount.Module.LeftPanel.Criteria
{
    using System.Collections;
    using Fanex.Data.Repository;

    public class GetBalanceInfoCriteria : CriteriaBase
    {
        public int CustId { get; set; }

        public GetBalanceInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey()
        {
            return "EAS_CustomerSelBalanceInfo";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}

