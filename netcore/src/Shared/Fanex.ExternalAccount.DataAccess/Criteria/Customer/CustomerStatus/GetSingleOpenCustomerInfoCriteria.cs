namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class GetSingleOpenCustomerInfoCriteria : CriteriaBase
    {
        public GetSingleOpenCustomerInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey() => "Acc_Customer_CloseInfoSel";

        public override bool IsValid() => CustId > 0;
    }
}