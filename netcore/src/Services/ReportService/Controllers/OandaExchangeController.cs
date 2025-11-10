using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (OandaExchangeRate Read)
    public class OandaExchangeController : ControllerBase
    {
        private readonly IExchangeService exchangeService;

        public OandaExchangeController(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            // TODO: Implement proper view rendering or API response
            return Ok(new { message = "Oanda Exchange Index" });
        }

        [HttpPost]
        [Route("GetExchangeRate")]
        public IActionResult GetExchangeRate([FromBody] GetExchangeRateRequest request)
        {
            var exchangeRates = exchangeService.GetOandaExchangeRate(request.FromDate, request.ToDate, request.GroupName);
            return Ok(exchangeRates);
        }
    }

    // TODO: Define proper interface and service
    public interface IExchangeService
    {
        object GetOandaExchangeRate(DateTime fromDate, DateTime toDate, string groupName);
    }

    public class ExchangeService : IExchangeService
    {
        public object GetOandaExchangeRate(DateTime fromDate, DateTime toDate, string groupName)
        {
            // TODO: Implement actual service logic
            return new[] { new { currency = "USD", rate = 1.0m, date = DateTime.Now } };
        }
    }

    // TODO: Define proper request model
    public class GetExchangeRateRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string GroupName { get; set; } = string.Empty;
    }
}
