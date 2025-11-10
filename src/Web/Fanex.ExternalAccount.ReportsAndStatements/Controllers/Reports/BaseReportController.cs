using Microsoft.AspNetCore.Mvc;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class BaseReportController : Controller
    {
        protected readonly CurrentSession currentSession;
        protected readonly Fanex.ExternalAccount.ReportsAndStatements.Services.Reports.IReportService reportService;

        protected BaseReportController(
            CurrentSession currentSession,
            Fanex.ExternalAccount.ReportsAndStatements.Services.Reports.IReportService reportService)
        {
            this.currentSession = currentSession ?? new CurrentSession();
            this.reportService = reportService;
        }

        protected bool CanView(string[] groups)
            => groups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);
    }

    // Placeholder classes - these need to be properly migrated from the original codebase
    public class CurrentSession
    {
        public int CustId { get; set; }
        public string ParentGroupName { get; set; } = string.Empty;
    }
}
