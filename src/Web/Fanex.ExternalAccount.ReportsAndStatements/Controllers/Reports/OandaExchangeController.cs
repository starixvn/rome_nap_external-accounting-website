using Microsoft.AspNetCore.Mvc;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class OandaExchangeController : Controller
    {
        private readonly IExchangeService exchangeService;

        public OandaExchangeController() : this(null) { }

        public OandaExchangeController(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService ?? new ExchangeService();
        }

        public IActionResult Index()
        {
            return View();
        }
    }

    // Placeholder interfaces
    public interface IExchangeService { }
    public class ExchangeService : IExchangeService { }
}
