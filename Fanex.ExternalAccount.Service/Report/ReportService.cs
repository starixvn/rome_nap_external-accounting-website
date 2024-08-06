namespace Fanex.ExternalAccount.Service.Impl.Report
{
    using Caching;
    using DataAccess.Impl.Report;
    using DataAccess.Interface.Report;
    using DTO.Customer;
    using DTO.Report;
    using Fanex.Caching.Memory;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Products;
    using Interface.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Utilities;

#pragma warning disable S109 // Magic numbers should not be used

    public class ReportService : IReportService
    {
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";
        private static readonly ICacheService Cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        private readonly IReportDao _reportDao;
        private readonly IProductService _productService;
        private readonly IProductDetailService _productDetailService;

        public ReportService(
            IReportDao reportRespository = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null)
        {
            _reportDao = reportRespository ?? new ReportDao();
            _productService = productService ?? new ProductService();
            _productDetailService = productDetailService ?? new ProductDetailService();
        }

        public static DateTime BalanceUpDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);

        public static DateTime MoveBetsDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);

        public DataTable GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
            => _reportDao.EAS_CustomerSelUserInfo(groupname, custId, userName, isGetUplineInfo);

        public LineInfo SearchCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
            => _reportDao.GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);

        public List<SuggestedUserName> GetSuggestedUserName(int custId, string username, string groupName)
            => _reportDao.EAS_CustomerSelUserSearch(custId, username, groupName);

        public List<DailyWinLoss> GetDailyWinLoss(DateTime winlostDate, int custId, string groupName)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_Com(winlostDate, custId, groupName)
                : _reportDao.EAS_Rpt_WinLossDaily_Com14(winlostDate, custId, groupName);

        public List<DailyWinLoss> GetDailyWinLossSuper(DateTime winlostDate, int custId, int masterId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_SAg(winlostDate, custId, masterId)
                : _reportDao.EAS_Rpt_WinLossDaily_SAg14(winlostDate, custId, masterId);

        public List<DailyWinLoss> GetDailyWinLossMaster(DateTime winlostDate, int custId, int agentId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_MAg(winlostDate, custId, agentId)
                : _reportDao.EAS_Rpt_WinLossDaily_MAg14(winlostDate, custId, agentId);

        public List<DailyWinLoss> GetDailyWinLossAgent(DateTime winlostDate, int custId, int memberId)
            => CheckConstraintMoveBetsDay(winlostDate)
                ? _reportDao.EAS_Rpt_WinLossDaily_Ag(winlostDate, custId, memberId)
                : _reportDao.EAS_Rpt_WinLossDaily_Ag14(winlostDate, custId, memberId);

        public List<WinLossDetailExtra> GetSuperWinLossDetail(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_Com(groupname, custId, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public List<WinLossDetail> GetMasterWinLossDetail(
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_SAg(custId, masterId, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public List<WinLossDetailMAg> GetAgentWinLossDetail(
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_MAg(custId, agentId, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public List<WinLossDetailAg> GetMemberWinLossDetail(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML)
            => _reportDao.EAS_Rpt_WinLossDetail_Ag(custId, memberId, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public List<WinLossDetailIBCBetLiveCasino> GetSuperWinLossDetailIBCBetLiveCasino(
            string groupname,
            DateTime fromDate,
            DateTime toDate)
            => _reportDao.EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(groupname, fromDate, toDate, MoveBetsDate);

        public ArrayList GetSortingOrderList(string groupname)
            => _reportDao.EAS_Rpt_WinLossDaily_SortingOrder(groupname);

        public List<ExtraPTModel> GetExtraPT(string groupName) => _reportDao.EAS_Rpt_ViewExtraPT2(groupName);

        public static DataTable GetBalanceUpDay()
        {
            Cache.TryGet<DataTable>(BalanceUpDayKey, out var balanceUpDay);
            if (balanceUpDay == null)
            {
                var reportDao = new ReportDao();
                balanceUpDay = reportDao.Age_SelBalanceUpDay();
                if (balanceUpDay != null)
                {
                    Cache.Set<DataTable>(BalanceUpDayKey, balanceUpDay, new CacheItemOptions { RelativeExpiration = TimeSpan.FromMinutes(5) });
                }
            }

            return balanceUpDay;
        }

        private bool CheckConstraintMoveBetsDay(DateTime fromDate)
            => DateTime.Compare(fromDate, MoveBetsDate) > 0;

        public string BuildTooltipsForLiveCasino(int userId, string groupName, int groupId)
        {
            var products = _productService
                .GetProducts(userId, groupName, groupId);
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
            var products = _productService.GetProducts(userId, groupName, groupId);
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