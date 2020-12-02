namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Linq;
    using Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Service.Impl.Report;
    using Service.Interface.Report;

    public class BaseReportController : BaseController
    {
        protected readonly CurrentSession currentSession;
        protected readonly IReportService reportService;

        protected BaseReportController(
            CurrentSession currentSession,
            IReportService reportService)
        {
            this.currentSession = currentSession ?? new CurrentSession();
            this.reportService = reportService ?? new ReportService();
        }

        protected bool CanView(string[] groups)
            => groups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);

    }
}