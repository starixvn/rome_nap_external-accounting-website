namespace Fanex.ExternalAccount.MatchWinLossReports.Services
{
    using System;
    using System.Linq;
    using System.Collections.Generic;
    using Criteria;
    using DataAccesses;
    using Models;

    public interface IMatchWinLossReportsService
    {
        SummaryReportResult GetSummaryReportData(SummaryReportQuery query);

        DetailReportResult GetDetailReportResult(DetailReportQuery query);

        IEnumerable<SportModel> GetSports(int? productId);

        IEnumerable<LeagueModel> GetLeagues(int sportId, string groupName, DateTime fromDate, DateTime toDate);

        IEnumerable<MatchModel> GetEvents(int sportId, string groupName, string leagueIds, DateTime fromDate);

        IEnumerable<BetTypeModel> GetBetTypes();

        IEnumerable<CurrencyModel> GetCurrencies(string groupName);
    }

    public class MatchWinLossReportsService : IMatchWinLossReportsService
    {
        private readonly IMatchWinLossReportsDataAccess _dataAccess;
        

        public MatchWinLossReportsService(IMatchWinLossReportsDataAccess dataAccess = null)
        {
            _dataAccess = dataAccess ?? new MatchWinLossReportsDataAccess();
        }

        public SummaryReportResult GetSummaryReportData(SummaryReportQuery query)
        {
            var dbResult = _dataAccess.GetSummaryReportData(new GetSummaryReportDataCriteria()
            {
                FromDate = query.FromDateString,
                ToDate = query.ToDateString,
                PageSize = query.PageSize,
                Page = query.Page,
                GroupName = query.GroupName,

                SportType = query.Sport,
                LeagueIds = query.Leagues,
                Currencies = query.Currencies,
                BaseCurrency = query.BaseCurrency,
                SortType = query.SortType,
                SortBy = query.SortBy
            });

            return dbResult;
        }

        public DetailReportResult GetDetailReportResult(DetailReportQuery query)
        {
            var dbResult = _dataAccess.GetDetailReportData(new GetDetailReportDataCriteria
            {
                FromDate = query.FromDateString,
                PageSize = query.PageSize,
                Page = query.Page,
                GroupName = query.GroupName,

                SportType = query.Sport,
                LeagueIds = query.Leagues,
                Currencies = query.Currencies,
                BaseCurrency = query.BaseCurrency,
                SortType = query.SortType,
                SortBy = query.SortBy,

                BetTypes = query.BetTypes,
                MatchIds = query.Matches
            });

            dbResult.Events = new List<ReportRawEntity>();
            if (dbResult.Mains.Count != 0)
            {
                for (int i = 0; i < dbResult.Mains.Count; i++)
                {
                    var match1 = dbResult.Mains[i];
                    var match2Id = i < dbResult.Mains.Count - 1 ? dbResult.Mains[i + 1].MatchId : 0;
                    var subTotal = dbResult.SubTotals.First(e => e.MatchId == match1.MatchId);
                    match1.KickOffTime = subTotal.KickOffTime;
                    match1.KickOffTimeStr = subTotal.KickOffTimeStr;
                    match1.HomeId = subTotal.HomeId;
                    match1.AwayId = subTotal.AwayId;
                    match1.IsNeutral = subTotal.IsNeutral;
                    match1.LeagueId = subTotal.LeagueId;
                    dbResult.Events.Add(match1);
                    if (match1.MatchId != match2Id)
                    {
                        dbResult.Events.Add(subTotal);
                    }
                }
            }
            dbResult.GroupLeagues = dbResult.Mains.GroupBy(e=>e.LeagueId).Select(e=>e.First()).ToList();
            return dbResult;
        }
        
        public IEnumerable<SportModel> GetSports(int? productId)
            => _dataAccess.GetSports(new SportsCriteria { ProductId = productId });

        public IEnumerable<LeagueModel> GetLeagues(int sportId, string groupName, DateTime fromDate, DateTime toDate)
            => _dataAccess.GetLeagues(new LeaguesCriteria
            {
                SportType = sportId,
                GroupName = groupName,
                FromDate = fromDate.ToString("MM/dd/yyyy"),
                ToDate = toDate.ToString("MM/dd/yyyy")
            });

        public IEnumerable<MatchModel> GetEvents(int sportId, string groupName, string leagueIds, DateTime fromDate)
            => _dataAccess.GetMatches(new MatchCriteria
            {
                SportType = sportId,
                GroupName = groupName,
                FromDate = fromDate.ToString("MM/dd/yyyy"),
                LeagueIds = leagueIds
            });

        public IEnumerable<BetTypeModel> GetBetTypes() => _dataAccess.GetBetTypes(new BetTypeCriteria());


        public IEnumerable<CurrencyModel> GetCurrencies(string groupName)
            => _dataAccess.GetCurrencies(new CurrencyCriteria { GroupName = groupName, });
    }
}