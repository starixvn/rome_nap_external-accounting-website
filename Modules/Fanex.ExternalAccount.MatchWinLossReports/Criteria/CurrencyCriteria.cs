namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    //using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;
    public class CurrencyCriteria: CriteriaBase
    {
        public string GroupName { get; set; }
        public override string GetSettingKey()
        {
            return "EAS_GetCurrencyList";
        }

        public override bool IsValid() => !string.IsNullOrEmpty(GroupName);
    }
}