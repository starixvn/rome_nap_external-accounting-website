namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    public class SearchMemberListCriteria : SearchCustomerListBaseCriteria
    {
        public int Recommend { get; set; }

        public override bool IsValid() => Recommend > 0 && base.IsValid();

        public override string GetSettingKey() => "EAS_CustomerSelByAg";
    }
}
