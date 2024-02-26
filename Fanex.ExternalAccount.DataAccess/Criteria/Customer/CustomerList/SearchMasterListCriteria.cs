namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    public class SearchMasterListCriteria : SearchCustomerListBaseCriteria
    {
        public int SRecommend { get; set; }

        public bool IsNoDownlines { get; set; }

        public override bool IsValid() => SRecommend > 0 && base.IsValid();

        public override string GetSettingKey() => "EAS_CustomerSelBySAg";
    }
}
