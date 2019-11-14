namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Linq;
    using Core.Base;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;

    public class BaseReportController : BaseController
    {
        protected readonly ICurrentSession currentSession;
        protected readonly IReportService reportService;
        protected readonly IGroupConfigurationService groupConfigurationService;

        protected BaseReportController(
            ICurrentSession currentSession,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
        {
            this.currentSession = currentSession ?? CurrentSession;
            this.reportService = reportService ?? new ReportService();
            this.groupConfigurationService = groupConfigurationService ?? new GroupConfigurationService();
        }
    }
}