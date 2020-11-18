namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
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

            var validProductIds = productService.ValidateProductIds(request.UserId, request.GroupId, ToEnumerable(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = getWinLossDetail(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                string.Join(",", validProductIds),
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

            var validProductIds = productService.ValidateProductIds(request.UserId, request.GroupId, ToEnumerable(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ResourceIds, request.GroupName);

            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ResourceIds, request.GroupName);

            var winLossDetailData = SearchWinLossDetailData(request.GroupName, request.FromDate, request.ToDate, string.Join(",", validProductIds), productDetailsXML, customerInfo);

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

            var extraPTs = reportService.GetExtraPT(request.GroupName);
            var validProductIds = productService
                .ValidateProductIds(request.UserId, request.GroupId, extraPTs.Select(e => e.ProductId))
                .ToDictionary(kvp => kvp, kvp => true);

            return extraPTs
                .Select(item => new { ExtraPT = item.ExtraPT * percentage, item.ProductName, item.ProductId })
                .Where(item => !string.IsNullOrWhiteSpace(item.ProductName) && validProductIds.ContainsKey(item.ProductId))
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

        public string ConcatProductName(string productIds, string resourceIds, string groupName)
        {
            var productIdList = ToEnumerable(productIds);

            if (string.IsNullOrWhiteSpace(resourceIds)
                && Config.IsOfferJackpotProductDetailsGroup(groupName))
            {
                productIdList = productIdList.Where(id => id != ProductConstants.PragmaticPlay);
            }

            return ConcatProductName(productIdList, resourceIds, groupName);
        }

        private string ConcatProductName(IEnumerable<int> productIds, string resourceIds, string groupName)
        {
            var productNames = new List<string>();

            productIds.ToList().ForEach(productId =>
            {
                if (productId == ProductConstants.PragmaticPlay)
                {
                    var userSelectedResourceIds = ToEnumerable(resourceIds).ToList();
                    var resourceIdList = productDetailService.GetProductDetails(groupName)
                        .Where(p => p.ProductId == ProductConstants.PragmaticPlay)
                        .Select(p => int.Parse(p.ResourceId))
                        .ToList();

                    if (resourceIdList.Count > 0 && !IsEquals(resourceIdList, userSelectedResourceIds))
                    {
                        productNames.AddRange(userSelectedResourceIds.Select(id => CacheRef.GetProductDetailName(id)));
                    }
                    else
                    {
                        productNames.Add(CacheRef.GetProductName(productId));
                    }
                }
                else
                {
                    productNames.Add(CacheRef.GetProductName(productId));
                }
            });

            return string.Join(", ", productNames);
        }

        private static IEnumerable<int> ToEnumerable(string value)
        {
            if (!string.IsNullOrWhiteSpace(value))
            {
                var arrayString = value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var item in arrayString)
                {
                    if (int.TryParse(item, out var intValue))
                    {
                        yield return intValue;
                    }
                }
            }
        }

        private static bool IsEquals(IEnumerable<int> source, IEnumerable<int> destination)
            => Enumerable.SequenceEqual(source.OrderBy(id => id), destination.OrderBy(id => id));

        public string BuildTooltipsForLiveCasino(int userId, int groupId)
        {
            var offeredLiveCasino = productService
                .GetProducts(userId, groupId)
                .FirstOrDefault(p => p.ProductId == ProductConstants.DummyLiveCasinoProductId);

            if (offeredLiveCasino != null && offeredLiveCasino.IsParent)
            {
                var ignoreProductIds = new Dictionary<int, bool>
                {
                    [ProductConstants.GoldDeluxe] = true
                };

                var liveCasinoName = string.Join(
                    ", ",
                    offeredLiveCasino.Products.Where(p => !ignoreProductIds.ContainsKey(p.ProductId)).Select(p => p.ProductName));

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }
    }
}