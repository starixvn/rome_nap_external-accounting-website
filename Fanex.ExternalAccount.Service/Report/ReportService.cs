namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using Caching;
    using DataAccess.Impl.Report;
    using DataAccess.Interface.Report;
    using DTO.Customer;
    using DTO.Report;
    using Fanex.Caching.Memory;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    using Fanex.ExternalAccount.Service.Products;
    using Interface.Report;
    using Microsoft.Extensions.Logging;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Utilities;
    using static StackExchange.Redis.Role;

#pragma warning disable S109 // Magic numbers should not be used

    public class ReportService : IReportService
    {
        private readonly IReportDao _reportDao;
        private readonly IProductService _productService;
        private readonly IProductDetailService _productDetailService;
        private readonly IReportUtils _reportUtils;

        public ReportService(
            IReportDao reportRespository = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null,
            IReportUtils reportUtils = null)
        {
            _reportDao = reportRespository ?? new ReportDao();
            _productService = productService ?? new ProductService();
            _productDetailService = productDetailService ?? new ProductDetailService();
            _reportUtils = reportUtils ?? new ReportUtils();
        }

        public static DateTime BalanceUpDate => new ReportUtils().BalanceUpDate;

        public LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
            => _reportDao.GetCustomerInfo(new GetCustomerInfoCriteria(groupname, custId, userName, isGetUplineInfo));

        public IEnumerable<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
            => _reportDao.EAS_CustomerSelUserSearch(new GetSuggestUsernamesCriteria(custId, username, groupName));

        public IEnumerable<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
            => _reportDao.EAS_Rpt_WinLossDaily_Com(new GetCompanyDailyWinlossCriteria(winlostDate, custId, groupName, _reportUtils.MoveBetsDate));

        public IEnumerable<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId, int memberId)
            => _reportDao.EAS_Rpt_WinLossDaily_SAg(new GetSuperDailyWinlossCriteria(winlostDate, custId, masterId, _reportUtils.MoveBetsDate));

        public IEnumerable<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId, int memberId)
            => _reportDao.EAS_Rpt_WinLossDaily_MAg(new GetMasterDailyWinlossCriteria(winlostDate, custId, agentId, _reportUtils.MoveBetsDate));

        public IEnumerable<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
            => _reportDao.EAS_Rpt_WinLossDaily_Ag(new GetAgentDailyWinlossCriteria(winlostDate, custId, memberId, _reportUtils.MoveBetsDate));

        public IEnumerable<WinLossDetailExtra> GetSuperWinLossDetail(
            string groupname,
            int? custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_Com(new GetSuperWinlossDetailCriteria
            {
                GroupName = groupname,
                CustId = custId > 0 ? custId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<WinLossDetail> GetMasterWinLossDetail(
            int custId,
            int? masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_SAg(new GetMasterWinlossDetailCriteria
            {
                CustId = custId,
                MasterId = masterId > 0 ? masterId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<WinLossDetail> GetDirectMemberOfSuperWinLossDetail(
            int custId,
            int? memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_SAg(new GetMasterWinlossDetailCriteria
            {
                CustId = custId,
                MemberId = memberId > 0 ? memberId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<WinLossDetailMAg> GetAgentWinLossDetail(
            int custId,
            int? agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_MAg(new GetAgentWinlossDetailCriteria
            {
                CustId = custId,
                AgentId = agentId > 0 ? agentId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<WinLossDetailMAg> GetDirectMemberOfMasterWinLossDetail(
            int custId,
            int? memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_MAg(new GetAgentWinlossDetailCriteria
            {
                CustId = custId,
                MemberId = memberId > 0 ? memberId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<WinLossDetailAg> GetMemberWinLossDetail(
            int custId,
            int? memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_Ag(new GetMemberWinlossDetailCriteria
            {
                CustId = custId,
                MemberId = memberId > 0 ? memberId : null,
                FromDate = fromDate,
                ToDate = toDate,
                ProductIds = productIds,
                ProductDetailsXML = productDetailsXML,
                BalanceUpDay = _reportUtils.BalanceUpDate,
                MoveBetsDay = _reportUtils.MoveBetsDate
            });

        public IEnumerable<int> GetSortingOrderList(string groupname)
            => _reportDao.EAS_Rpt_WinLossDaily_SortingOrder(new GetSortingOrderCriteria(groupname));

        public IEnumerable<ExtraPTModel> GetExtraPT(string groupName) => _reportDao.EAS_Rpt_ViewExtraPT2(new GetExtraPTCriteria(groupName));

        public string BuildTooltipsForLiveCasino(int userId, string groupName, int groupId)
        {
            var products = _productService
                .GetProducts(groupName, groupId);
            var offeredLiveCasino = products
                .FirstOrDefault(p => p.ProductId == ProductConstants.DummyLiveCasinoProductId);
            var offeredLiveCasinoFGG = products
                .FirstOrDefault(p => p.ProductId == ProductConstants.FGG);

            if (offeredLiveCasino != null && offeredLiveCasino.IsParent)
            {
                var liveCasinoName = Utils.JoinToString(offeredLiveCasino.Products.Select(p => p.ProductName), ", ");
                var fggName = offeredLiveCasinoFGG != null ? $", {offeredLiveCasinoFGG.ProductName}" : string.Empty;

                liveCasinoName = !string.IsNullOrWhiteSpace(liveCasinoName) ? $"{liveCasinoName}{fggName}" : $"{fggName}";

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }

        public string BuildTooltipsForRNGCasino(int userId, string groupName, int groupId)
        {
            var products = _productService.GetProducts(groupName, groupId);
            var childProducts = products
                .FirstOrDefault(p => p.ProductId == ProductConstants.DummyRNGProductId)
                .Products.Select(p => p.ProductName);
            var parentProduct = products
                .Where(p => ProductConstants.ProductListForTooltipsRNGCasino.Contains(p.ProductId))
                .Select(p => p.ProductName);
            var offeredRNGCasino = parentProduct.Concat(childProducts);

            var rngCasinoName = Utils.JoinToString(offeredRNGCasino, ", ");
            return string.Format(ViewLang.Tooltip_NetTurnover_RNGCasino, rngCasinoName);
        }

        public string ConcatProductName(string productIds, string resourceIds, string groupName)
        {
            var productIdList = Utils.ToEnumerableInt(productIds);
            var productOfferProductDetails = Config.ProductOfferProductDetails();
            if (string.IsNullOrWhiteSpace(resourceIds) && Config.IsOfferProductDetailGroup(groupName))
            {
                productIdList = productIdList.Where(id => !productOfferProductDetails.Contains(id) && id != ProductConstants.Sportsbook);
            }

            return ConcatProductName(productIdList, resourceIds, groupName);
        }

        private string ConcatProductName(IEnumerable<int> productIds, string resourceIds, string groupName)
        {
            var productDetailNames = GetProductDetailNames(Utils.ToEnumerableInt(resourceIds), groupName);
            var productNames = ConcatProductNames(productIds, productDetailNames);

            return productNames;
        }

        private IDictionary<int, string> GetProductDetailNames(IEnumerable<int> selectedResourceIds, string groupName)
        {
            var productNames = new Dictionary<int, string>();

            var details = _productDetailService.GetProductDetails(groupName)
                .GroupBy(p => p.ResourceId)
                .Select(p =>
                {
                    return p.FirstOrDefault();
                });

            details
            .GroupBy(p => p.ProductId)
            .ToList()
            .ForEach(group =>
            {
                var allResourceIds = group.Select(p => int.Parse(p.ResourceId)).ToList();
                var intersectResourceIds = selectedResourceIds.Intersect(allResourceIds).ToList();

                if (intersectResourceIds.Count > 0
                    && intersectResourceIds.Count != allResourceIds.Count
                    && selectedResourceIds.Any())
                {
                    productNames[group.Key] = string.Join(", ", intersectResourceIds.Select(id => CacheRef.GetProductDetailName(id)));
                }
            });

            return productNames;
        }

        private static string ConcatProductNames(IEnumerable<int> productIds, IDictionary<int, string> productNames)
           => productIds == null
           ? string.Empty
           : string.Join(", ", productIds.Select(id => productNames.TryGetValue(id, out var name) ? name : CacheRef.GetProductName(id)));
#pragma warning restore S109 // Magic numbers should not be used
    }
}