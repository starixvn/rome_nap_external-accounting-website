using Fanex.Data.Repository;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetEventsCriteria : CriteriaBase
    {
        public GetEventsCriteria(string leagueIds)
        {
            LeagueIds = leagueIds;
        }

        public string LeagueIds { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_EventSel";

        public override bool IsValid() => true;
    }
}
