using System.Collections.Generic;
using System.Linq;
using Fanex.Data.Repository;
using Fanex.ExternalAccount.Core.Shared;
using Fanex.ExternalAccount.DataAccess.Criteria.TotalBet;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.DTO.TotalBet;
using Fanex.ExternalAccount.Service.Params;

namespace Fanex.ExternalAccount.Service.TotalBet
{
    public class TotalBetService : ITotalBetService
    {
        private readonly IDynamicRepository dynamicRepository;

        public TotalBetService() : this(null)
        {
        }

        public TotalBetService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<LeagueModel> GetLeagues(bool isTest)
        {
            var criteria = new GetLeaguesCriteria(isTest);
            var data = dynamicRepository.Fetch<LeagueModel>(criteria).ToList();

            foreach (var d in data)
            {
                d.LeagueName = CacheRef.GetLeagueName(d.LeagueId.ToString());
            }

            return data;
        }

        public IEnumerable<EventModel> GetEvents(string leagueIds)
        {
            var criteria = new GetEventsCriteria(leagueIds);
            var data = dynamicRepository.Fetch<EventModel>(criteria).ToList();

            foreach (var d in data)
            {
                d.MatchName = CacheRef.GetTeamName(d.HomeId.ToString()) + (d.IsNeutral ? " (N)" : string.Empty) + " -vs- " +
                              CacheRef.GetTeamName(d.AwayId.ToString());
            }

            return data;
        }

        public IEnumerable<TotalBetLeagueModel> GetData(GetTotalBetParam param)
        {
            var criteria = new GetTotalBetCriteria(
                param.GroupName, param.ViewMode,
                string.Join(",", param.Leagues),
                string.Join(",", param.Events),
                param.BaseCurrencyName
            );
            var leagues = dynamicRepository.Fetch<TotalBetModel>(criteria).GroupBy(d => d.LeagueId);
            var result = new List<TotalBetLeagueModel>();

            foreach (var league in leagues)
            {
                var events = new List<TotalBetModel>();
                foreach (var d in league)
                {
                    d.HDPFT = RoundNumber(d.HDPFT);
                    d.OUFT = RoundNumber(d.OUFT);
                    d.HDPHT = RoundNumber(d.HDPHT);
                    d.OUHT = RoundNumber(d.OUHT);

                    d.IsLatest = !league.Any(x => x.MatchId == d.MatchId && x.TotalScore > d.TotalScore);
                    d.HomeName = CacheRef.GetTeamName(d.HomeId.ToString());
                    d.AwayName = CacheRef.GetTeamName(d.AwayId.ToString());
                    events.Add(d);
                }

                result.Add(new TotalBetLeagueModel
                {
                    LeagueId = league.Key,
                    LeagueName = CacheRef.GetLeagueName(league.Key.ToString()),
                    Events = events
                });
            }

            return result;
        }

        private static decimal? RoundNumber(decimal? d)
            => d.HasValue ? (decimal?)int.Parse(d.Value.ToString("N0").Replace(",", string.Empty)) : null;
    }
}