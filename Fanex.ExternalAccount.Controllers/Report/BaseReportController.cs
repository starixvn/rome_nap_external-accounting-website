namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System;
    using System.Linq;

    public class BaseReportController : BaseController
    {
        protected readonly CurrentSession currentSession;
        protected readonly IReportService reportService;
        protected readonly IGroupConfigurationService groupConfigurationService;

        protected BaseReportController(
            CurrentSession currentSession,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
        {
            this.currentSession = currentSession ?? new CurrentSession();
            this.reportService = reportService ?? new ReportService();
            this.groupConfigurationService = groupConfigurationService ?? new GroupConfigurationService();
        }

        protected bool CanView(string[] groups)
        {
            return groups.Contains(currentSession.ParentGroupName, StringComparer.CurrentCultureIgnoreCase);
        }
    }
}