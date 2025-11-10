using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (TotalBetSoccerBaseCurrency Read)
    public class TotalBetMCController : ControllerBase
    {
        protected readonly ICurrentSession currentSession;
        private readonly ITotalBetService totalBetService;

        public TotalBetMCController(
            ICurrentSession currentSession,
            ITotalBetService totalBetService)
        {
            this.currentSession = currentSession;
            this.totalBetService = totalBetService;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            // TODO: var testGroups = Config.TestGroups();
            var testGroups = new[] { "testgroup1", "testgroup2" }; // Placeholder
            var isTest =
                testGroups.Contains(currentSession.CurrencyGroup, StringComparer.CurrentCultureIgnoreCase) ||
                testGroups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

            var leagues = totalBetService.GetLeagues(isTest);

            var filters = new TotalBetFilterViewModel { Leagues = leagues };

            return Ok(filters);
        }

        [HttpGet]
        [Route("GetEvents")]
        public IActionResult GetEvents([FromQuery] string leagueIds)
        {
            var events = string.IsNullOrWhiteSpace(leagueIds)
                ? Array.Empty<EventModel>()
                : totalBetService.GetEvents(leagueIds);

            return Ok(events);
        }

        [HttpPost]
        [Route("GetData")]
        public IActionResult GetData([FromBody] TotalBetMCRequest request)
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

            return Ok(totalBetService.GetData(param));
        }
    }

    // TODO: Define proper request model
    public class TotalBetMCRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public string ViewMode { get; set; } = string.Empty;
        public string Leagues { get; set; } = string.Empty;
        public string Events { get; set; } = string.Empty;
        public string BaseCurrency { get; set; } = string.Empty;
        public string BaseCurrencyName { get; set; } = string.Empty;
    }
}