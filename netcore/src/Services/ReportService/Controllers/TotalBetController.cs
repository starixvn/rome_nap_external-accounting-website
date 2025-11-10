using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (TotalBetSoccer Read)
    public class TotalBetController : ControllerBase
    {
        protected readonly ICurrentSession currentSession;
        private readonly ITotalBetService totalBetService;

        public TotalBetController(
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
        public IActionResult GetData([FromBody] TotalBetRequest request)
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

    // TODO: Define proper interfaces and services
    public interface ITotalBetService
    {
        object GetLeagues(bool isTest);
        object GetEvents(string leagueIds);
        object GetData(GetTotalBetParam param);
    }

    public class TotalBetService : ITotalBetService
    {
        public object GetLeagues(bool isTest)
        {
            // TODO: Implement actual service logic
            return new[] { new { id = 1, name = "Premier League" } };
        }

        public object GetEvents(string leagueIds)
        {
            // TODO: Implement actual service logic
            return new[] { new { id = 1, name = "Manchester United vs Liverpool" } };
        }

        public object GetData(GetTotalBetParam param)
        {
            // TODO: Implement actual service logic
            return new { data = "total bet data", param = param };
        }
    }

    // TODO: Define proper request/response models
    public class TotalBetRequest
    {
        public string GroupName { get; set; } = string.Empty;
        public string ViewMode { get; set; } = string.Empty;
        public string Leagues { get; set; } = string.Empty;
        public string Events { get; set; } = string.Empty;
    }

    public class GetTotalBetParam
    {
        public string GroupName { get; set; } = string.Empty;
        public string ViewMode { get; set; } = string.Empty;
        public string Leagues { get; set; } = string.Empty;
        public string Events { get; set; } = string.Empty;
    }

    public class TotalBetFilterViewModel
    {
        public object Leagues { get; set; } = null!;
    }

    public class EventModel
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}