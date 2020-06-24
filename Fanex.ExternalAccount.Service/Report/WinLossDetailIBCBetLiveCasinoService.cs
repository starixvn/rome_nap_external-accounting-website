namespace Fanex.ExternalAccount.Service.Impl.Report
{
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

            var winLossDetailIBCBetLiveCasinoData =
                SearchWinLossDetailIBCBetLiveCasinoData(
                    request.GroupName,
                    request.FromDate,
                    request.ToDate);

            return new
            {
                rows = winLossDetailIBCBetLiveCasinoData,
                sortingorderlist = sortingOrderList
            };
        }

        private dynamic SearchWinLossDetailIBCBetLiveCasinoData(
            string groupname,
            DateTime fromDate,
            DateTime toDate)
        {
            return reportService.GetSuperWinLossDetailIBCBetLiveCasino(
                    groupname,
                    fromDate,
                    toDate);
        }
    }
}