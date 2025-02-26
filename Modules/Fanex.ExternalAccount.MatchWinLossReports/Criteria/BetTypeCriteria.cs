namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    // using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;
    public class BetTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_BettypeSel";
        }

        public override bool IsValid() => true;
    }
}