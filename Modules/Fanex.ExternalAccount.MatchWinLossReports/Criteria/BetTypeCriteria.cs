namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    using Fanex.NAP.Framework.Data;
    
    public class BetTypeCriteria : CriteriaBase
    {
        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_BettypeSel";
        }
    }
}