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
            var balanceUpDay = Cache.Get<DataTable>(BalanceUpDayKey);
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

        public string BuildTooltipsForLiveCasino(int userId, int groupId)
        {
            var offeredLiveCasino = _productService
                .GetProducts(userId, groupId)
                .FirstOrDefault(p => p.ProductId == ProductConstants.DummyLiveCasinoProductId);

            if (offeredLiveCasino != null && offeredLiveCasino.IsParent)
            {
                var ignoreProductIds = new Dictionary<int, bool>
                {
                    [ProductConstants.GoldDeluxe] = true
                };

                var liveCasinoName = Utils.JoinToString(offeredLiveCasino.Products.Where(p => !ignoreProductIds.ContainsKey(p.ProductId)).Select(p => p.ProductName), ", ");

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }

        public string ConcatProductName(string productIds, string resourceIds, string groupName)
        {
            var productIdList = Utils.ToEnumerableInt(productIds);

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
                    var userSelectedResourceIds = Utils.ToEnumerableInt(resourceIds).ToList();
                    var resourceIdList = _productDetailService.GetProductDetails(groupName)
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

            return Utils.JoinToString(productNames, ", ");
        }

        private static bool IsEquals(IEnumerable<int> source, IEnumerable<int> destination)
            => Enumerable.SequenceEqual(source.OrderBy(id => id), destination.OrderBy(id => id));
    }

#pragma warning restore S109 // Magic numbers should not be used
}