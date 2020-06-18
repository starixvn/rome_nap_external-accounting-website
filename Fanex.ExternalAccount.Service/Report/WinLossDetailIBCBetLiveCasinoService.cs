namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using Core.Shared;
    using DTO.Customer;
    using Interface.Report;
    using System;

    public class WinLossDetailIBCBetLiveCasinoService : IWinLossDetailIBCBetLiveCasinoService
    {
        private readonly IReportService reportService;

        public WinLossDetailIBCBetLiveCasinoService()
        {
            reportService = new ReportService();
        }

        public dynamic SearchWinLossDetailIBCBetLiveCasino(ISearchWinLossDetailIBCBetLiveCasinoRequest request)
        {
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);

            var customerInfo = new LineInfo { RoleId = Role.Super };

            var winLossDetailIBCBetLiveCasinoData =
                SearchWinLossDetailIBCBetLiveCasinoData(
                    request.GroupName,
                    request.FromDate,
                    request.ToDate,
                    request.ProductIds,
                    customerInfo);

            return new
            {
                viewlevel = customerInfo.RoleId,
                supername = customerInfo.SuperName,
                mastername = customerInfo.MasterName,
                agentname = customerInfo.AgentName,
                srecommend = customerInfo.SuperId,
                mrecommend = customerInfo.MasterId,
                recommend = customerInfo.AgentId,
                rows = winLossDetailIBCBetLiveCasinoData,
                sortingorderlist = sortingOrderList,
                tooltips = request.ToolTipsProducts
            };
        }

        private dynamic SearchWinLossDetailIBCBetLiveCasinoData(
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            LineInfo customerInfo)
        {
            return reportService.GetSuperWinLossDetailIBCBetLiveCasino(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
        }
    }
}