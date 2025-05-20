using Fanex.Data.Repository;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetTotalBetCriteria : CriteriaBase
    {
        public GetTotalBetCriteria(string groupName, int viewMode, string leagueIds, string matchIds, string baseCurrencyName)
        {
            GroupName = groupName;
            ViewMode = viewMode;
            LeagueIds = leagueIds;
            MatchIds = matchIds;
            BaseCurrency = baseCurrencyName;
        }

        

        public string GroupName { get; set; }
        public int ViewMode { get; set; }
        public string LeagueIds { get; set; }
        public string MatchIds { get; set; }
        public string BaseCurrency { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_1378Sel";

        public override bool IsValid() => true;
    }
}