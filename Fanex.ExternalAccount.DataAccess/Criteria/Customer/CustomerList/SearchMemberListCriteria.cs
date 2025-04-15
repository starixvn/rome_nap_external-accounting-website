namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    public class SearchMemberListCriteria : SearchCustomerListBaseCriteria
    {
        public int UplineID { get; set; }

        public override bool IsValid() => UplineID > 0 && base.IsValid();

        public override string GetSettingKey() => "EAS_CustomerSelByAg";
    }
}
