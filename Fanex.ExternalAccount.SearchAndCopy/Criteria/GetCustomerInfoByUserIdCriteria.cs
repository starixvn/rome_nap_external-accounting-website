namespace Fanex.ExternalAccount.SearchAndCopy.Criteria
{
    using Fanex.NAP.Framework.Data;

    public class GetCustomerInfoByUserIdCriteria : CriteriaBase
    {
        public GetCustomerInfoByUserIdCriteria(int custId, bool isMember)
        {
            CustId = custId;
            IsMember = isMember;
        }

        public int CustId { get; }

        [SpParam(Ignored = true)]
        public bool IsMember { get; }

        public override string GetSettingKey()
            => IsMember ? "Acc_CustomerSelEditCustomer" : "Acc_CustomerSelCEditCustomer";

        public override bool IsValid() => CustId > 0;
    }
}
