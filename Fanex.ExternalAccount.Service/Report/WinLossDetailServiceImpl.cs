namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using Caching;
    using Core.Shared;
    using DTO.Customer;
    using Interface.Report;

    public class WinLossDetailServiceImpl : WinLossDetailService
    {
        private readonly IReportService reportService;
        private readonly ICache cacheManager;

        public WinLossDetailServiceImpl(IReportService reportService = null, ICache cacheManager = null)
        {
            this.reportService = reportService ?? new ReportService();
            this.cacheManager = cacheManager ?? CacheService.CreateInstance();
        }

        private Func<int, int, DateTime, DateTime, string, object> GetWinLossDetailFunction(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return reportService.GetMasterWinLossDetail;

                case Role.Master:
                    return reportService.GetAgentWinLossDetail;

                case Role.Agent:
                    return reportService.GetMemberWinLossDetail;

                default:
                    throw new InvalidOperationException(nameof(roleId));
            }
        }

        public dynamic DrillDownWinLossDetail(IDrillDownWinLossDetailRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);
            var getWinLossDetail = GetWinLossDetailFunction(request.ViewLevel);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                request.ProductIds);

            if (winLossDetailData != null)
            {
                var cacheKey = request.GetCacheKey();
                cacheManager.Set(cacheKey, winLossDetailData);
            }

            return
                new
                {
                    viewlevel = request.ViewLevel.DirectDownlineLevel(),
                    srecommend = request.SuperId,
                    mrecommend = request.MasterId,
                    recommend = request.AgentId,
                    rows = winLossDetailData,
                    sortingorderlist = sortingOrderList,
                    tooltips = request.ToolTipsProducts
                };
        }

        public dynamic SearchWinLossDetail(ISearchWinLossDetailRequest request)
        {
            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);

            var customerInfo = new LineInfo { RoleId = Role.Super };

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                customerInfo = reportService.SearchCustomerInfo(request.GroupName, 0, request.Username, true);

                if (customerInfo == null)
                {
                    return NoInfoResult(request.Username, sortingOrderList, request.ToolTipsProducts);
                }
            }

            var winLossDetailData = SearchWinLossDetailData(request.GroupName, request.FromDate, request.ToDate, request.ProductIds, customerInfo);

            if (winLossDetailData != null)
            {
                var cacheKey = request.GetCacheKey();

                cacheManager.Set(cacheKey, winLossDetailData);
            }

            return
                new
                {
                    viewlevel = customerInfo.RoleId,
                    supername = customerInfo.SuperName,
                    mastername = customerInfo.MasterName,
                    agentname = customerInfo.AgentName,
                    srecommend = customerInfo.SuperId,
                    mrecommend = customerInfo.MasterId,
                    recommend = customerInfo.AgentId,
                    rows = winLossDetailData,
                    sortingorderlist = sortingOrderList,
                    tooltips = request.ToolTipsProducts
                };
        }

        private dynamic SearchWinLossDetailData(
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportService.GetSuperWinLossDetail(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }

            var getWinLossDetail = GetWinLossDetailFunction(customerInfo.RoleId.DirectUplineLevel());

            return getWinLossDetail(
                customerInfo.GetUplineId(),
                customerInfo.CustId,
                fromDate,
                toDate,
                productIds);
        }

        private static dynamic NoInfoResult(string username, ArrayList sortingOrderList, dynamic toolTipsProducts) => new
        {
            viewlevel = Role.Super,
            supername = username,
            mastername = string.Empty,
            agentname = string.Empty,
            rows = string.Empty,
            sortingorderlist = sortingOrderList,
            tooltips = toolTipsProducts
        };
    }
}