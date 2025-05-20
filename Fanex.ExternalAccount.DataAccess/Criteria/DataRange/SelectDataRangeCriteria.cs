namespace Fanex.ExternalAccount.DataAccess.Criteria.DataRange
{
    using Fanex.Data.Repository;

    public class SelectDataRangeCriteria : CriteriaBase
    {
        public string CustId { get; set; }

        public override string GetSettingKey() => "EAS_CustomerDateLimitSel";

        public override bool IsValid() => !string.IsNullOrWhiteSpace(CustId);
    }
}