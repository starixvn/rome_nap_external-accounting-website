using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class SuperWinLossDetailByPTController : BaseReportController
    {
        public SuperWinLossDetailByPTController(
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
