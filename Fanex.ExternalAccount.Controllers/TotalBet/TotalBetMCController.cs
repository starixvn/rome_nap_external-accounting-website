namespace Fanex.ExternalAccount.Controllers.TotalBet
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Params;
    using Fanex.ExternalAccount.Service.TotalBet;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.TotalBet;
    using Requests;

    [ExternalAccountAuthenticate(Function = eFunctions.TotalBetSoccerBaseCurrency, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class TotalBetMCController : BaseController
    {
        protected readonly CurrentSession currentSession;
        private readonly ITotalBetService totalBetService;

        public TotalBetMCController(
            CurrentSession currentSession,
            ITotalBetService totalBetService)
        {
            this.currentSession = currentSession ?? new CurrentSession();
            this.totalBetService = totalBetService ?? new TotalBetService();
        }

        public TotalBetMCController() : this(null, null)
        {
        }

        public ActionResult Index()
        {
            var testGroups = Config.TestGroups();
            var isTest =
                testGroups.Contains(currentSession.CurrencyGroup, StringComparer.CurrentCultureIgnoreCase) ||
                testGroups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

            var leagues = totalBetService.GetLeagues(isTest);

            var filters = new TotalBetFilterViewModel { Leagues = leagues };

            return View(filters);
        }

        public ActionResult GetEvents(string leagueIds)
        {
            var events = string.IsNullOrWhiteSpace(leagueIds)
                ? Enumerable.Empty<EventModel>()
                : totalBetService.GetEvents(leagueIds);

            return Json(events);
        }

        public ActionResult GetData(TotalBetMCRequest request)
        {
            var param = new GetTotalBetParam
            {
                GroupName = request.GroupName,
                ViewMode = request.ViewMode,
                Leagues = request.Leagues,
                Events = request.Events,
                BaseCurrency = request.BaseCurrency,
                BaseCurrencyName = request.BaseCurrencyName
            };

            return Json(totalBetService.GetData(param));
        }
    }
}