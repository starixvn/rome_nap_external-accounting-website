using System;
using System.Collections.Generic;
using System.Linq;
using Fanex.Caching;
using Fanex.Caching.Memory;
using Fanex.CustomerSetting.Module.Core.Services.Impl;
using Fanex.ExternalAccount.DataAccess.Criteria.TotalBet;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.DTO.TotalBet;
using Fanex.ExternalAccount.Service.Params;
using Fanex.NAP.Framework.Data;

namespace Fanex.ExternalAccount.Service.TotalBet
{
    public class TotalBetService : ITotalBetService
    {
        private const string EventsCachedKey = "Fanex.ExternalAccount.Service.Report.TotalBet.EventsCachedKey";
        private const int EventsCachedTimeInMinutes = 2;

        private static readonly ICacheService Cache
            = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        private readonly IDynamicRepository dynamicRepository;

        public TotalBetService() : this(null) { }

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
                d.LeagueName = CacheRef.GetLeagueName(d.LeagueId);
            }

            return data;
        }

        public IEnumerable<EventModel> GetEvents(int leagueId)
        {
            return Cache.GetOrSet($"{EventsCachedKey}-{leagueId}",
                () =>
                {
                    var criteria = new GetEventsCriteria(leagueId);
                    var data = dynamicRepository.Fetch<EventModel>(criteria).ToList();

                    foreach (var d in data)
                    {
                        d.MatchName = CacheRef.GetTeamName(d.HomeId) + (d.IsNeutral ? " (N)" : string.Empty) + " -vs- " + CacheRef.GetTeamName(d.AwayId);
                    }

                    return data;
                },
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromMinutes(EventsCachedTimeInMinutes)));
        }

        public IEnumerable<TotalBetModel> GetData(GetTotalBetParam param)
        {
            var criteria = new GetTotalBetCriteria
            {
                GroupName = param.GroupName,
                ViewMode = param.ViewMode,
                LeagueIds = param.League.ToString(),
                MatchIds = string.Join(",", param.Events),
            };

            var data = dynamicRepository.Fetch<TotalBetModel>(criteria).ToList();

            foreach (var d in data)
            {
                d.HDPFT = RoundNumber(d.HDPFT);
                d.OUFT = RoundNumber(d.OUFT);
                d.HDPHT = RoundNumber(d.HDPHT);
                d.OUHT = RoundNumber(d.OUHT);

                d.IsLatest = !data.Any(x => x.MatchId == d.MatchId && x.TotalScore > d.TotalScore);
                d.HomeName = CacheRef.GetTeamName(d.HomeId);
                d.AwayName = CacheRef.GetTeamName(d.AwayId);
            }

            return data;
        }

        private static decimal? RoundNumber(decimal? d)
            => d.HasValue ? (decimal?)int.Parse(d.Value.ToString("N0").Replace(",", string.Empty)) : null;

        public IEnumerable<TotalBetModel> GetDataMC(GetTotalBetMCParam param)
        {
            var criteria = new GetTotalBetMCCriteria
            {
                GroupName = param.GroupName,
                ViewMode = param.ViewMode,
                LeagueIds = param.League.ToString(),
                MatchIds = string.Join(",", param.Events),
                BaseCurrency = param.BaseCurrency
            };

            var data = dynamicRepository.Fetch<TotalBetModel>(criteria).ToList();

            foreach (var d in data)
            {
                d.HDPFT = RoundNumber(d.HDPFT);
                d.OUFT = RoundNumber(d.OUFT);
                d.HDPHT = RoundNumber(d.HDPHT);
                d.OUHT = RoundNumber(d.OUHT);

                d.IsLatest = !data.Any(x => x.MatchId == d.MatchId && x.TotalScore > d.TotalScore);
                d.HomeName = CacheRef.GetTeamName(d.HomeId);
                d.AwayName = CacheRef.GetTeamName(d.AwayId);
            }

            return data;
        }
    }
}
