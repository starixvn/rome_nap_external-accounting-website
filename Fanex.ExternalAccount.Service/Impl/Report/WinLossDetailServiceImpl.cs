using System;

namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System.Collections;
    using System.Collections.Generic;
    using Interface.Report;
    public class WinLossDetailServiceImpl : WinLossDetailService
    {
        private readonly IReportService reportService;
        private readonly Dictionary<int, Func<int, int, DateTime, DateTime, string, object>>
            GetWinLossDetailFunctions;

        public WinLossDetailServiceImpl(IReportService reportService)
        {
            this.reportService = reportService ?? new ReportService();

            GetWinLossDetailFunctions =
                new Dictionary<int, Func<int, int, DateTime, DateTime, string, object>>
                {
                    {4, this.reportService.GetWinLossDetailSuper},
                    {3, this.reportService.GetWinLossDetailMAg},
                    {2, this.reportService.GetWinLossDetailAg}
                };
        }

        public dynamic DrillDownWinLossDetail(IDrillDownRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }

            var getWinLossDetail = GetWinLossDetailFunctions[request.ViewLevel];

            var winLossData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                request.ProductIds);

            return winLossData;
        }

        //public SearchWinLossDetail(
        //    string username,
        //    string groupname,
        //    DateTime fromDate,
        //    DateTime toDate,
        //    string productIds)
        //{
        //    var sortingOrderList = reportService.GetSortingOrderList(groupname);
        //    var customerInfo = new CustomerInfoSearchResult { RoleId = Role.Super };

        //    if (!string.IsNullOrWhiteSpace(username))
        //    {
        //        customerInfo = _reportService.SearchCustomerInfo(groupname, 0, username, true);

        //        if (customerInfo == null)
        //        {
        //            return NoInfoResult(username, sortingOrderList);
        //        }
        //    }

        //    object winLossDetailData;

        //    if (customerInfo.RoleId >= Role.Super)
        //    {
        //        winLossDetailData = reportService.GetWinLossDetail(
        //            groupname,
        //            customerInfo.CustId,
        //            fromDate,
        //            toDate,
        //            productIds);
        //    }
        //    else
        //    {
        //        var getWinLossDetail = GetWinLossDetailFunctions[customerInfo.RoleId.DirectUplineLevel()];

        //        winLossDetailData = getWinLossDetail(
        //            customerInfo.GetCustomerUplineId(),
        //            customerInfo.CustId,
        //            fromDate,
        //            toDate,
        //            productIds);
        //    }
        //}
    }
}
