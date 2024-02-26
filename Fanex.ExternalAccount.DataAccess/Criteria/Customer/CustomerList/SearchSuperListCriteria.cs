namespace Fanex.ExternalAccount.DataAccess.Criteria.Customer.CustomerList
{
    public class SearchSuperListCriteria : SearchCustomerListBaseCriteria
    {
        public string GroupName { get; set; }

        public bool IsDataLimited { get; set; }

        public bool IsNoDownlines { get; set; }

        public override bool IsValid() => !string.IsNullOrWhiteSpace(GroupName) && base.IsValid();

        public override string GetSettingKey() => "EAS_CustomerSelByCom";
    }
}
