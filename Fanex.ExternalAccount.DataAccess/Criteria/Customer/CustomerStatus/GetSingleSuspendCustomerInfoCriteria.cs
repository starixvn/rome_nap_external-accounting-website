namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using Fanex.Data.Repository;

    public class GetSingleSuspendCustomerInfoCriteria: CriteriaBase
    {
        public GetSingleSuspendCustomerInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey()
        {
            return "Acc_Customer_SuspendInfoSel";
        }

        public override bool IsValid()
        {
            return CustId > 0;
        }
    }
}
