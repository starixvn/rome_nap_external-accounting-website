namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using Fanex.Data.Repository;

    public class GetCurrencyListCriteria : ICriteria
    {
        public GetCurrencyListCriteria(string groupName) => GroupName = groupName;
        public string GroupName { get; set; }

        public string GetSettingKey() => "EAS_GetCurrencyList";

        public bool IsValid() => true;
    }
}