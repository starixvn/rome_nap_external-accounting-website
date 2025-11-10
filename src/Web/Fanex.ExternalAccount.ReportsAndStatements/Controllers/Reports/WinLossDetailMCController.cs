using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class WinLossDetailMCController : BaseReportController
    {
        public WinLossDetailMCController(
            CurrentSession currentSession,
            IReportService reportService)
            : base(currentSession, reportService)
        {
        }

        public IActionResult Index()
        {
            return View();
        }
    }
}
