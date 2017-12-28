namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using Caching;
    using Common.Extensions;
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

        public dynamic DrillDownWinLossDetail(IDrillDownRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }

            var getWinLossDetail = GetWinLossDetailFunction(request.ViewLevel);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                request.ProductIds);

            if (winLossDetailData != null)
            {
                var cacheKey = "CacheList" + request.GroupName + request.CustId + request.FromDate + request.ToDate + request.ProductIds;

                cacheManager.Set(cacheKey, winLossDetailData);
            }

            return winLossDetailData;
        }

        public dynamic SearchWinLossDetail(
            CustomerInfoSearchResult customerInfo,
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds
            )
        {
            object winLossDetailData;

            if (customerInfo.RoleId >= Role.Super)
            {
                winLossDetailData = reportService.GetSuperWinLossDetail(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }
            else
            {
                var getWinLossDetail = GetWinLossDetailFunction(customerInfo.RoleId.DirectUplineLevel());

                winLossDetailData = getWinLossDetail(
                    customerInfo.GetUplineId(),
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }

            if (winLossDetailData != null)
            {
                var cacheKey = "CacheList" + customerInfo.Username + groupname + "0" +/* customerInfo.CustId + */fromDate + toDate +
                               productIds;

                cacheManager.Set(cacheKey, winLossDetailData);
            }

            return winLossDetailData;
        }
    }
}