namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Linq;
    using Core.Base;
    using Core.Reports.WinLossDetail;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using Service.Impl.Report;
    using Service.Interface.Report;

    public class BaseReportController : BaseController
    {
        protected readonly ICurrentSession currentSession;
        protected readonly IReportService reportService;

        protected BaseReportController(ICurrentSession currentSession, IReportService reportService)
        {
            this.currentSession = currentSession ?? CurrentSession;
            this.reportService = reportService ?? new ReportService();
        }

        protected bool IsShowIncludePockerGames(string groupName)
            => WinLossDetailHelper.IsShowProduct("showPG", currentSession)
            && reportService.GetExcludeProductIds(groupName).Contains(ProductConstants.PokerGames);
    }
}