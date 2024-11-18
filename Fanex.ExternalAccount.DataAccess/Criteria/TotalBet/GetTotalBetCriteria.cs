using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetTotalBetCriteria : CriteriaBase
    {
        private readonly string[] mappingBaseCurrency = { "MYR", "CNY", "THB", "IDR" };

        public GetTotalBetCriteria(string groupName, int viewMode, string leagueIds, string matchIds, int baseCurrency)
        {
            GroupName = groupName;
            ViewMode = viewMode;
            LeagueIds = leagueIds;
            MatchIds = matchIds;
            BaseCurrency = mappingBaseCurrency[baseCurrency - 1];
        }

        public string GroupName { get; set; }
        public int ViewMode { get; set; }
        public string LeagueIds { get; set; }
        public string MatchIds { get; set; }
        public string BaseCurrency { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_1378Sel";
    }
}