namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Base;
    using CustomerSetting.Module.Core.Web;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System;

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

        protected bool IsIBCINDO()
        {
            const string IBCINDO = "IBCINDO";

            string groupName = RouteData.Values["id"] as string;
            if (string.IsNullOrWhiteSpace(groupName))
            {
                groupName = Core.Context.CurrentSession.Instance.CurrencyGroup;
            }
            if (!groupName.Equals(IBCINDO, StringComparison.CurrentCultureIgnoreCase))
            {
                return false;
            }

            return true;
        }
    }
}
