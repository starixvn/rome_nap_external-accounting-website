namespace Fanex.ExternalAccount.Controllers.TotalBet
{
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
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
    using Core.Authorization;
    using Microsoft.Extensions.Configuration;

    [ExternalAccountAuthenticate(Function = eFunctions.TotalBetSoccer, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class TotalBetController : BaseController
    {
        private readonly ITotalBetService totalBetService;

        public TotalBetController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            IAuthorizationService authorizationService,
            ITotalBetService totalBetService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.totalBetService = totalBetService;
        }

        public IActionResult Index()
        {
            var testGroups = Config.TestGroups();
            var isTest =
                testGroups.Contains(CurrentSession.CurrencyGroup, StringComparer.CurrentCultureIgnoreCase) ||
                testGroups.Contains(CurrentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

            var leagues = totalBetService.GetLeagues(isTest);

            var filters = new TotalBetFilterViewModel { Leagues = leagues };

            return View(filters);
        }

        public IActionResult GetEvents(string leagueIds)
        {
            var events = string.IsNullOrWhiteSpace(leagueIds)
                ? Enumerable.Empty<EventModel>()
                : totalBetService.GetEvents(leagueIds);

            return Ok(events);
        }

        public IActionResult GetData(TotalBetRequest request)
        {
            var param = new GetTotalBetParam
            {
                GroupName = request.GroupName,
                ViewMode = request.ViewMode,
                Leagues = request.Leagues,
                Events = request.Events,
            };

            return Ok(totalBetService.GetData(param));
        }
    }
}