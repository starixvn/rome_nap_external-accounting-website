namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Linq;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;

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