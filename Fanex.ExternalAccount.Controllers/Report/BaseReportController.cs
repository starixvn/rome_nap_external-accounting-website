namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Linq;
    using Core.Base;
    using Core.Shared;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Utilities.Reports.WinLossDetail;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System.Collections.Generic;

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
            var groupName = currentSession.ParentGroupName;

            return groups.Contains(groupName, StringComparer.CurrentCultureIgnoreCase);
        }

        protected string BuildTooltipsForLiveCasino(string groupName)
        {
            var liveCasino = new List<GroupConfigurationModel>
            {
                new GroupConfigurationModel{ProductId=ProductConstants.AllBet,ProductName="AllBet"},
                new GroupConfigurationModel{ProductId=ProductConstants.SexyGaming,ProductName="AE Casino"},
                new GroupConfigurationModel{ProductId=ProductConstants.SAGaming,ProductName="SA Gaming"},
                new GroupConfigurationModel{ProductId=ProductConstants.GTCasino,ProductName="IBCBet Live Casino"},
                new GroupConfigurationModel{ProductId=ProductConstants.BBin,ProductName="BBIN"},
                new GroupConfigurationModel{ProductId=ProductConstants.ION,ProductName="ION"},
                new GroupConfigurationModel{ProductId=ProductConstants.WM,ProductName="WM"}
            };

            var offeredLiveCasino = liveCasino.Where(
                i => groupConfigurationService.IsShowByProductAndGroup(i.ProductId, groupName));

            if (offeredLiveCasino != null && offeredLiveCasino.Any())
            {
                var liveCasinoName = string.Join(", ", offeredLiveCasino.Select(i => i.ProductName));

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
    }
}