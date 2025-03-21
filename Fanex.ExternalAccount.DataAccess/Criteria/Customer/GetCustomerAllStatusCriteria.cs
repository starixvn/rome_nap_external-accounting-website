namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer
{
    using Fanex.Data.Repository;

    public class GetCustomerAllStatusCriteria: CriteriaBase
    {
        public int CustId { get; set; }

        public byte SettingStatus { get; set; }

        public override bool IsValid() => CustId > 0;

        public override string GetSettingKey() => "EAS_CustomerSelAllStatus";
    }
}
