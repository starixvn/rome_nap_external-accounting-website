namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    using Fanex.Data.Repository;
    public class SportsCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "EAS_MatchWinloss_SportSel";

        public override bool IsValid() => true;
    }
}