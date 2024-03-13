namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    public class SearchAgentListCriteria : SearchCustomerListBaseCriteria
    {
        public int MRecommend { get; set; }

        public bool IsNoDownlines { get; set; }
        public override bool IsValid() => MRecommend > 0 && base.IsValid();

        public override string GetSettingKey() => "EAS_CustomerSelByMAg";
    }
}
