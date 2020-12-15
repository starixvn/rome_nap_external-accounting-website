namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Linq;
    using Core.Shared;
    using DTO.Customer;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;
    using Interface.Report;

    public class WinLossDetailServiceImpl : WinLossDetailService
    {
        private readonly IReportService reportService;
        private readonly IProductService productService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailServiceImpl(
            IReportService reportService = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null)
        {
            this.reportService = reportService ?? new ReportService();
            this.productService = productService ?? new ProductService();
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

            var validProductIds = productService.ValidateProductIds(request.UserId, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                Utils.ToString(validProductIds),
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

            var validProductIds = productService.ValidateProductIds(request.UserId, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = SearchWinLossDetailData(request.GroupName, request.FromDate, request.ToDate, Utils.ToString(validProductIds), productDetailsXML, customerInfo);

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

        public dynamic GetExtraPT(IViewExtraPTRequest request)
        {
            const float percentage = 100.0f;

            var extraPTs = reportService.GetExtraPT(request.ParentGroupName);
            var validProductIds = productService
                .ValidateProductIds(request.UserId, request.GroupId, extraPTs.Select(e => e.ProductId))
                .ToDictionary(kvp => kvp, kvp => true);

            return extraPTs
                .Select(item => new { ExtraPT = item.ExtraPT * percentage, item.ProductName, item.ProductId })
                .Where(item => !string.IsNullOrWhiteSpace(item.ProductName) && validProductIds.ContainsKey(item.ProductId))
                .GroupBy(item => item.ExtraPT, item => item.ProductName,
                (key, group) => new { ExtraPT = key, ProductNames = Utils.ToString(group, ", ") })
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