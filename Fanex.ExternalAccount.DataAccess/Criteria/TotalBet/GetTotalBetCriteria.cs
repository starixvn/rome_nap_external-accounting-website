using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetTotalBetCriteria : CriteriaBase
    {
        public string GroupName { get; set; }
        public int ViewMode { get; set; }
        public string LeagueIds { get; set; }
        public string MatchIds { get; set; }
        public int BaseCurrency { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_1378Sel";
    }
}
