namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetSuperInAutomateTransferCriteria : CriteriaBase
    {
        public int CustId { get; }

        public GetSuperInAutomateTransferCriteria(int custId)
        {
            CustId = custId;
        }

        public override string GetSettingKey() => "Acc_CustomerSelSuperAutoTransfer";

        public override bool IsValid() => CustId > 0;
    }
}

