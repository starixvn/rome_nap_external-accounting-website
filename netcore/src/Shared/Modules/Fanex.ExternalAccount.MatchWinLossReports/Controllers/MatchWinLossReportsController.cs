namespace Fanex.ExternalAccount.MatchWinLossReports.Controllers
{
    using System;
    using Microsoft.AspNetCore.Mvc;
    using System.Linq;
    using Services;
    using Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.MatchWinLossReports.Models;
    using Fanex.ExternalAccount.MatchWinLossReports.ViewModels;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Shared;
    using Caching;
    using Fanex.Caching.Memory;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.MatchWinLoss,
        Policy = AuthenticateAction.AuthenticationType.Read)]
    public class MatchWinLossReportsController : BaseController
    {
        private readonly IMatchWinLossReportsService service;
        private readonly ICacheService _cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));
        private const string SummaryReportCacheKey = "cacheMatchWinLossSummary";

        private const string DetailReportCacheKey = "cacheMatchWinLossDetail1";

        public MatchWinLossReportsController(IMatchWinLossReportsService service, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.service = service ?? new MatchWinLossReportsService();
            this._cache = new CacheService();
        }

        public IActionResult Index()
        {
            var today = DateTime.Now;
            var minDate = DateTime.Now.AddMonths(-3).AddDays((-DateTime.Now.Day) + 1);
            var sports = service.GetSports().ToList();
            var userGroup = CurrentSession.CurrencyGroup;
            var leagues = service.GetLeagues(sports[0].SportType, userGroup, today, today);
            var leagueIds = string.Join(", ", leagues.Select(x => x.LeagueId).ToList());
            var events = service.GetEvents(sports[0].SportType, userGroup, leagueIds, today);

            var filterInfo = new MatchWinLossReportsViewModels
            {
                Today = today,
                TodayString = today.ToString("MM/dd/yyyy"),
                MinDate = minDate,
                MinDateString = minDate.ToString("MM/dd/yyyy"),
                Sports = sports,
                Leagues = leagues,
                Currencies = service.GetCurrencies(userGroup),
                Events = events,
                BetTypes = service.GetBetTypes(),
                IsMY = Config.IsMalaysiaGroup(userGroup)
            };
            return PartialViewBase(filterInfo);
        }

        public IActionResult Details() => PartialViewBase();

        public IActionResult Summary() => PartialViewBase();

        public IActionResult GetLeagues(int sportId, DateTime fromDate, DateTime toDate)
        {
            var leagues = service.GetLeagues(sportId, CurrentSession.CurrencyGroup, fromDate, toDate);
            return Json(leagues);
        }

        public IActionResult GetLeaguesAndEvents(int sportId, DateTime fromDate, DateTime toDate)
        {
            var leagues = service.GetLeagues(sportId, CurrentSession.CurrencyGroup, fromDate, toDate);
            var leagueIds = string.Join(",", leagues.Select(obj => obj.LeagueId).ToList());
            var events = service.GetEvents(sportId, CurrentSession.CurrencyGroup, leagueIds, fromDate);
            return Json(new { leagues, events });
        }

        public IActionResult GetEvents(int sportId, string leagueIds, DateTime fromDate)
        {
            var events = service.GetEvents(sportId, CurrentSession.CurrencyGroup, leagueIds, fromDate);
            return Json(events);
        }

        public IActionResult GetSummaryReport(SummaryReportQuery query)
        {
            query.GroupName = CurrentSession.CurrencyGroup;
            var data = service.GetSummaryReportData(query);
            _cache.Set(SummaryReportCacheKey, query, new CacheItemOptions());
            return Json(data);
        }

        public IActionResult GetDetailReport(DetailReportQuery query)
        {
            query.GroupName = CurrentSession.CurrencyGroup;
            var data = service.GetDetailReportResult(query);
            _cache.Set(DetailReportCacheKey, query, new CacheItemOptions());
            return Json(data);
        }

        public void ExportSummaryReport()
        {
            _cache.TryGet<SummaryReportQuery>(SummaryReportCacheKey, out var cachedRequest);
            cachedRequest.Page = 1;
            cachedRequest.PageSize = int.MaxValue;
            cachedRequest.IsMY = Config.IsMalaysiaGroup(CurrentSession.CurrencyGroup);
            
            var data = service.GetSummaryReportData(cachedRequest);
            var excelBuilder = new MatchWinLossReportsExcel();

            using (var memoryStream = excelBuilder.BuildSummaryReportStream(data, cachedRequest))
            {
                ExportToExcel(memoryStream,
                    excelBuilder.GetSummaryReportFileName(cachedRequest.FromDate, cachedRequest.ToDate));
            }
        }
        
        public void ExportDetailReport()
        {
            _cache.TryGet<DetailReportQuery>(DetailReportCacheKey, out var cachedRequest);
            cachedRequest.Page = 1;
            cachedRequest.PageSize = int.MaxValue;
            cachedRequest.IsMY = Config.IsMalaysiaGroup(CurrentSession.CurrencyGroup);
            
            var data = service.GetDetailReportResult(cachedRequest);
            var excelBuilder = new MatchWinLossReportsExcel();

            using (var memoryStream = excelBuilder.BuildDetailReportStream(data, cachedRequest))
            {
                ExportToExcel(memoryStream, excelBuilder.GetDetailReportFileName(cachedRequest.FromDate));
            }
        }
    }
}