namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerStatus
{
    using NAP.Framework.Data;

    public class GetSingleOpenCustomerInfoCriteria : CriteriaBase
    {
        public GetSingleOpenCustomerInfoCriteria(int custId)
        {
            CustId = custId;
        }

        public int CustId { get; }

        public override string GetSettingKey() => "Acc_Customer_SuspendInfoSel";

        public override bool IsValid() => CustId > 0;
    }
}