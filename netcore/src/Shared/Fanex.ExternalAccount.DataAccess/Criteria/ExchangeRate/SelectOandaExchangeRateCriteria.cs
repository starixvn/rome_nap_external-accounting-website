namespace Fanex.ExternalAccount.DataAccess.Criteria.ExchangeRate
{
    using System;
    using Fanex.Data.Repository;

    public class SelectOandaExchangeRateCriteria : CriteriaBase
    {
        public string GroupName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }

        public override string GetSettingKey() => "EAS_Rpt_OandaExchangeRate";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(GroupName);
    }
}
