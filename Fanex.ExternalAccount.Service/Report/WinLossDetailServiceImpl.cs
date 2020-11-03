namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Linq;
    using Core.Shared;
    using DTO.Customer;
    using Fanex.ExternalAccount.Service.Products;
    using Interface.Report;

    public class WinLossDetailServiceImpl : WinLossDetailService
    {
        private readonly IReportService reportService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailServiceImpl(IReportService reportService = null, IProductDetailService productDetailService = null)
        {
            this.reportService = reportService ?? new ReportService();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        private Func<int, int, DateTime, DateTime, string, string, object> GetWinLossDetailFunction(int roleId)
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
            var productDetailsXML = productDetailService.BuildProductDetailsXML(request.ProductIds, request.ResourceIds, request.GroupName);
            var productIds = productDetailService.RemoveProductIds(request.ProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                productIds,
                productDetailsXML);

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

            var productDetailsXML = productDetailService.BuildProductDetailsXML(request.ProductIds, request.ResourceIds, request.GroupName);
            var productIds = productDetailService.RemoveProductIds(request.ProductIds, request.ResourceIds, request.GroupName);
            var winLossDetailData = SearchWinLossDetailData(request.GroupName, request.FromDate, request.ToDate, productIds, productDetailsXML, customerInfo);

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

        public dynamic GetExtraPT(IViewExtraPTRequest request, string groupName)
        {
            const float percentage = 100.0f;

            return reportService.GetExtraPT(groupName)
                .Select(item => new { ExtraPT = item.ExtraPT * percentage, ProductName = request.GetProductName(item.ProductId) })
                .Where(item => !string.IsNullOrWhiteSpace(item.ProductName))
                .GroupBy(item => item.ExtraPT, item => item.ProductName,
                (key, group) => new { ExtraPT = key, ProductNames = string.Join(", ", group) })
                .OrderBy(item => item.ExtraPT);
        }

        private dynamic SearchWinLossDetailData(
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportService.GetSuperWinLossDetail(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds,
                    productDetailsXML);
            }

            var getWinLossDetail = GetWinLossDetailFunction(customerInfo.RoleId.DirectUplineLevel());

            return getWinLossDetail(
                customerInfo.GetUplineId(),
                customerInfo.CustId,
                fromDate,
                toDate,
                productIds,
                productDetailsXML);
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