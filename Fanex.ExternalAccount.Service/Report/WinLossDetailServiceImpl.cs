namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Linq;
    using Core.Shared;
    using DTO.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;
    using Interface.Report;

    public class WinLossDetailServiceImpl : WinLossDetailService
    {
        private readonly IReportService reportService;
        private readonly IReportByCurrencyService reportByCurrencyService;
        private readonly IProductService productService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailServiceImpl(
            IReportService reportService = null,
            IReportByCurrencyService reportByCurrencyService = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null)
        {
            this.reportService = reportService ?? new ReportService();
            this.reportByCurrencyService = reportByCurrencyService ?? new ReportByCurrencyService();
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
        private Func<string, int, int, DateTime, DateTime, string, string, string, string, object> GetWinLossByCurrencyDetailFunction(int roleId)
        {
            switch (roleId)
            {
                case Role.Super:
                    return reportByCurrencyService.GetMasterWinLossMCDetail;

                case Role.Master:
                    return reportByCurrencyService.GetAgentWinLossMCDetail;

                case Role.Agent:
                    return reportByCurrencyService.GetMemberWinLossMCDetail;

                default:
                    throw new InvalidOperationException(nameof(roleId));
            }
        }
        
        public dynamic DrillDownWinLossDetail(IDrillDownWinLossDetailRequest request)
        {
            if (!request.IsValid)
            {
                return NoInfoResult("", new ArrayList(), request.ToolTipsProducts);
            }

            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);
            var getWinLossDetail = GetWinLossDetailFunction(request.ViewLevel);

            var validProductIds = productService.ValidateProductIds(request.GroupName, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                Utils.JoinToString(validProductIds),
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

        public dynamic DrillDownWinLossMCDetail(IDrillDownWinLossDetailRequest request)
        {
            if (!request.IsValid)
            {
                return NoInfoResult("", new ArrayList(), request.ToolTipsProducts);
            }

            var sortingOrderList = reportService.GetSortingOrderList(request.GroupName);
            var getWinLossDetail = GetWinLossByCurrencyDetailFunction(request.ViewLevel);

            var validProductIds = productService.ValidateProductIds(request.GroupName, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = getWinLossDetail(
                request.GroupName,
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                Utils.JoinToString(validProductIds),
                productDetailsXML,
                request.Currencies,
                request.BaseCurrencyName);

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

            if (!request.IsValid)
            {
                return NoInfoResult(request.Username, sortingOrderList, request.ToolTipsProducts);
            }

            if (!string.IsNullOrWhiteSpace(request.Username))
            {
                customerInfo = reportService.SearchCustomerInfo(request.GroupName, 0, request.Username, true);

                if (customerInfo == null)
                {
                    return NoInfoResult(request.Username, sortingOrderList, request.ToolTipsProducts);
                }
            }

            var validProductIds = productService.ValidateProductIds(request.GroupName, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);
            dynamic winLossDetailData = request.IsMultipleCurrency
                ? SearchWinLossMCDetailData(
                    request,
                    Utils.JoinToString(validProductIds),
                    productDetailsXML,
                    customerInfo)
                : SearchWinLossDetailData(
                    request,
                    Utils.JoinToString(validProductIds),
                    productDetailsXML,
                    customerInfo);
         
            return
                new
                {
                    viewlevel = GetViewLevel(customerInfo),
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

        private static int GetViewLevel(LineInfo customerInfo)
        {
            var dynamicRoleId = customerInfo.RoleId;
            if (customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfSuper())
            {
                dynamicRoleId = Role.Master;
            }
            if(customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfMaster())
            {
                dynamicRoleId = Role.Agent;
            }
            return dynamicRoleId;
        }

        public dynamic GetExtraPT(IViewExtraPTRequest request)
        {
            const float percentage = 100.0f;

            var extraPTs = reportService.GetExtraPT(request.ParentGroupName);
            var validProductIds = productService
                .ValidateProductIds(request.GroupName, request.GroupId, extraPTs.Select(e => e.ProductId))
                .ToDictionary(kvp => kvp, kvp => true);

            return extraPTs
                .Select(item => new { ExtraPT = item.ExtraPT * percentage, item.ProductName, item.ProductId })
                .Where(item => !string.IsNullOrWhiteSpace(item.ProductName) && validProductIds.ContainsKey(item.ProductId))
                .GroupBy(item => item.ExtraPT, item => item.ProductName,
                (key, group) => new { ExtraPT = key, ProductNames = Utils.JoinToString(group, ", ") })
                .OrderBy(item => item.ExtraPT);
        }

        private dynamic SearchWinLossDetailData(
            ISearchWinLossDetailRequest request,
            string productIds,
            string productDetailsXML,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportService.GetSuperWinLossDetail(
                    request.GroupName,
                    customerInfo.CustId,
                    request.FromDate,
                    request.ToDate,
                    productIds,
                    productDetailsXML);
            }
        
            var getWinLossDetail = GetWinLossDetailFunction(customerInfo.RoleId.DirectUplineLevel());
            var dynamicUplineId = customerInfo.GetUplineId();
            
            if (customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfSuper())
            {
                getWinLossDetail = reportService.GetDirectMemberOfSuperWinLossDetail;
                dynamicUplineId = customerInfo.SuperId;
            }
            
            if (customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfMaster())
            {
                getWinLossDetail = reportService.GetDirectMemberOfMasterWinLossDetail;
                dynamicUplineId = customerInfo.MasterId;
            }
            return getWinLossDetail(
                dynamicUplineId,
                customerInfo.CustId,
                request.FromDate,
                request.ToDate,
                productIds,
                productDetailsXML);
        }

        private dynamic SearchWinLossMCDetailData(
            ISearchWinLossDetailRequest request,
            string productIds,
            string productDetailsXML,
            LineInfo customerInfo)
        {
            if (customerInfo.RoleId >= Role.Super)
            {
                return reportByCurrencyService.GetSuperWinLossMCDetail(
                    request.GroupName,
                    customerInfo.CustId,
                    request.FromDate,
                    request.ToDate,
                    productIds,
                    productDetailsXML,
                    request.Currencies,
                    request.BaseCurrencyName);
            }
            var getWinLossDetail = GetWinLossByCurrencyDetailFunction(customerInfo.RoleId.DirectUplineLevel());
            var dynamicUplineId = customerInfo.GetUplineId();
            
            if (customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfSuper())
            {
                getWinLossDetail = reportByCurrencyService.GetDirectMemberOfSuperWinLossMCDetail;
                dynamicUplineId = customerInfo.SuperId;
            }
            
            if (customerInfo.RoleId.IsMember() && customerInfo.AgentId.IsDirectMemberOfMaster())
            {
                getWinLossDetail = reportByCurrencyService.GetDirectMemberOfMasterWinLossMCDetail;
                dynamicUplineId = customerInfo.MasterId;
            }

            return getWinLossDetail(
                request.GroupName,
                dynamicUplineId,
                customerInfo.CustId,
                request.FromDate,
                request.ToDate,
                productIds,
                productDetailsXML,
                request.Currencies,
                request.BaseCurrencyName);
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