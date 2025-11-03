namespace Fanex.ExternalAccount.MatchWinLossReports.Controllers
{
    using System;
    using System.Web.Mvc;
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

    [ExternalAccountAuthenticate(Function = eFunctions.MatchWinLoss,
        Policy = AuthenticateAction.AuthenticationType.Read)]
    public class MatchWinLossReportsController : BaseController
    {
        private readonly IMatchWinLossReportsService service;
        private readonly CurrentSession _currentSession;
        private readonly ICacheService _cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));
        private const string SummaryReportCacheKey = "cacheMatchWinLossSummary";

        private const string DetailReportCacheKey = "cacheMatchWinLossDetail1";

        public MatchWinLossReportsController() : this(null)
        {
            _cache = new CacheService();
            _currentSession = new CurrentSession();
        }

        public MatchWinLossReportsController(IMatchWinLossReportsService service)
        {
            this.service = service ?? new MatchWinLossReportsService();
        }

        public ActionResult Index()
        {
            var today = DateTime.Now;
            var minDate = DateTime.Now.AddMonths(-3).AddDays((-DateTime.Now.Day) + 1);
            var sports = service.GetSports().ToList();
            var userGroup = _currentSession.CurrencyGroup;
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

        public ActionResult Details() => PartialViewBase();

        public ActionResult Summary() => PartialViewBase();

        public ActionResult GetLeagues(int sportId, DateTime fromDate, DateTime toDate)
        {
            var leagues = service.GetLeagues(sportId, _currentSession.CurrencyGroup, fromDate, toDate);
            return Json(leagues, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetLeaguesAndEvents(int sportId, DateTime fromDate, DateTime toDate)
        {
            var leagues = service.GetLeagues(sportId, _currentSession.CurrencyGroup, fromDate, toDate);
            var leagueIds = string.Join(",", leagues.Select(obj => obj.LeagueId).ToList());
            var events = service.GetEvents(sportId, _currentSession.CurrencyGroup, leagueIds, fromDate);
            return Json(new { leagues, events }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetEvents(int sportId, string leagueIds, DateTime fromDate)
        {
            var events = service.GetEvents(sportId, _currentSession.CurrencyGroup, leagueIds, fromDate);
            return Json(events, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSummaryReport(SummaryReportQuery query)
        {
            query.GroupName = _currentSession.CurrencyGroup;
            var data = service.GetSummaryReportData(query);
            _cache.Set(SummaryReportCacheKey, query, new CacheItemOptions());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetDetailReport(DetailReportQuery query)
        {
            query.GroupName = _currentSession.CurrencyGroup;
            var data = service.GetDetailReportResult(query);
            _cache.Set(DetailReportCacheKey, query, new CacheItemOptions());
            return Json(data, JsonRequestBehavior.AllowGet);
        }

        public void ExportSummaryReport()
        {
            _cache.TryGet<SummaryReportQuery>(SummaryReportCacheKey, out var cachedRequest);
            cachedRequest.Page = 1;
            cachedRequest.PageSize = int.MaxValue;
            cachedRequest.IsMY = Config.IsMalaysiaGroup(_currentSession.CurrencyGroup);
            
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
            cachedRequest.IsMY = Config.IsMalaysiaGroup(_currentSession.CurrencyGroup);
            
            var data = service.GetDetailReportResult(cachedRequest);
            var excelBuilder = new MatchWinLossReportsExcel();

            using (var memoryStream = excelBuilder.BuildDetailReportStream(data, cachedRequest))
            {
                ExportToExcel(memoryStream, excelBuilder.GetDetailReportFileName(cachedRequest.FromDate));
            }
        }
    }
}