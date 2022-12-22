using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.TotalBet
{
    public class GetEventsCriteria : CriteriaBase
    {
        public GetEventsCriteria(int leagueId)
        {
            LeagueId = leagueId;
        }

        public int LeagueId { get; set; }

        public override string GetSettingKey() => "EAS_TotalBet_EventSel";
    }
}
