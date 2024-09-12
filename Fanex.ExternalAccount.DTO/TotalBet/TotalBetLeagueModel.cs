
using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO.TotalBet
{
    public class TotalBetLeagueModel
    {
        public int LeagueId { get; set; }
        public string LeagueName { get; set; }
        public IEnumerable<TotalBetModel> Events { get; set; }
    }
}
