using Microsoft.AspNetCore.Mvc;
using System;
using System.Linq;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    public class BaseReportController : ControllerBase
    {
        protected readonly ICurrentSession currentSession;
        protected readonly IReportService reportService;

        protected BaseReportController(
            ICurrentSession currentSession,
            IReportService reportService)
        {
            this.currentSession = currentSession;
            this.reportService = reportService;
        }

        protected bool CanView(string[] groups)
            => groups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);
    }

    // TODO: Define proper interfaces
    public interface ICurrentSession
    {
        string ParentGroupName { get; }
        // Add other properties as needed
    }

    public interface IReportService
    {
        // TODO: Define proper methods
    }

    public class CurrentSession : ICurrentSession
    {
        public string ParentGroupName => "default"; // TODO: Implement proper session logic
    }

    public class ReportService : IReportService
    {
        // TODO: Implement actual service logic
    }
}