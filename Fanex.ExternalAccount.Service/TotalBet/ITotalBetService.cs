using System.Collections.Generic;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.DTO.TotalBet;
using Fanex.ExternalAccount.Service.Params;

namespace Fanex.ExternalAccount.Service.TotalBet
{
    public interface ITotalBetService
    {
        IEnumerable<LeagueModel> GetLeagues(bool isTest);
        IEnumerable<EventModel> GetEvents(string leagueIds);
        IEnumerable<TotalBetLeagueModel> GetData(GetTotalBetParam param);
    }
}
