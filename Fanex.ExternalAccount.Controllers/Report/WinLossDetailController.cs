namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Caching;
    using Core.Base;
    using Core.Context;
    using Core.Reports;
    using Core.Reports.WinLossDetail;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class WinLossDetailController : BaseController
    {
        private const string Seperator = ", ";
        private const string DateFormat = "yyyyMMdd";
        private readonly ICache cacheManager;
        private readonly WinLossDetailService winLossDetailService;
        private readonly object ToolTipsProducts;
        private readonly Func<ICollection<KeyValuePair<string, bool>>> productQueryParamGetter;

        public WinLossDetailController(
            ICache cacheManager,
            ICurrentSession currentSession,
            WinLossDetailService winLossDetailService,
            Func<ICollection<KeyValuePair<string, bool>>> productQueryParamGetter)
        {
            this.cacheManager = cacheManager ?? CacheService.CreateInstance(CacheType.MemoryCache);
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();
            this.productQueryParamGetter = productQueryParamGetter ?? new FilterController().AsCollection;

            ToolTipsProducts = new
            {
                showAllbet = WinLossDetailHelper.IsShowProduct("showAB", currentSession ?? CurrentSession)
            };
        }

        public WinLossDetailController() : this(null, null, null, null)
        {
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index() => View();

        private string GroupName => CurrentSession.IsInternalAdmin
            ? CheckRequest.GetStringRequest("groupname", string.Empty)
            : CurrentSession.CurrencyGroup;

        private static string GetProductIds(ICollection<KeyValuePair<string, bool>> filterParams)
        {
            var formatedProductIds = new List<int>();

            foreach (ProductElementConfiguration item in ProductsConfiguration.Instance.ProductCollection)
            {
                foreach (var product in filterParams)
                {
                    if (item.ProductId != 0 && product.Key.Equals(item.Id, StringComparison.OrdinalIgnoreCase) && product.Value)
                    {
                        formatedProductIds.Add(item.ProductId);
                    }
                }
            }

            return string.Join(",", formatedProductIds);
        }

        private string GetProductNames(ICollection<KeyValuePair<string, bool>> filterParams)
        {
            var list = GetProductList();
            var formatedProducts = string.Empty;
            filterParams.ToList().ForEach(o =>
            {
                list.ForEach(p =>
                {
                    if (!p.Key.Equals("showAll", StringComparison.OrdinalIgnoreCase) && o.Value
                        && o.Key.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        formatedProducts += p.Value + Seperator;
                    }
                });
            });

            if (formatedProducts.Length > 0)
            {
                formatedProducts = formatedProducts.TrimEnd(Seperator.ToCharArray());
            }

            return formatedProducts;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailRequestParam request)
        {
            var viewlevel = request.viewlevel;
            var custId = request.custId;
            var fromDate = request.fromdate;
            var username = request.username;
            var toDate = request.todate;
            var productIds = GetProductIds(productQueryParamGetter());

            var viewModel =
                request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(username, GroupName, fromDate, toDate, productIds, ToolTipsProducts)
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownRequest(custId, viewlevel, fromDate, toDate, productIds, GroupName),
                    ToolTipsProducts);

            return Json(viewModel);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
          Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public void ExportWinLossDetailToExcel(WinLossDetailRequestParam request)
        {
            var productCollection = productQueryParamGetter();
            var fromDate = request.fromdate;
            var toDate = request.todate;
            var formatedProducts = GetProductNames(productCollection);
            var viewlevel = request.viewlevel;
            var productIds = GetProductIds(productCollection);


            var cacheKey = GetCacheKey(request, productIds);


            IWinLossDetailReportBuilder builder;

            switch (viewlevel)
            {
                case Role.Member:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetailAg>>(cacheKey);
                        builder = new MemberWinLossDetailReportBuilder(
                            winlossDetailData,
                            viewlevel,
                            fromDate,
                            toDate);

                        break;
                    }
                case Role.Agent:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetailMAg>>(cacheKey);
                        builder = new AgentWinLossDetailReportBuilder(
                            winlossDetailData,
                            viewlevel,
                            fromDate,
                            toDate);

                        break;
                    }
                default:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetail>>(cacheKey);
                        builder = new WinLossDetailReportBuilder(
                            winlossDetailData,
                            viewlevel,
                            fromDate,
                            toDate);

                        break;
                    }
            }

            var memoryStream = builder.Build(formatedProducts);

            var exportFileName =
                $"{viewlevel.DisplayRoleName()}WinLossDetail_{fromDate.ToString(DateFormat)}_{toDate.ToString(DateFormat)}.xls";

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, exportFileName);
                memoryStream.Dispose();
            }
        }

        private string GetCacheKey(WinLossDetailRequestParam request, string productIds)
        {
            return (request.custId > 0)
                ? string.Join(":", GroupName, request.custId, request.fromdate, request.todate, productIds)
                    .ToLowerInvariant()
                : string.Join(":", request.username, GroupName, request.fromdate, request.todate, productIds)
                    .ToLowerInvariant();
        }
    }

    public class DrillDownRequest : IDrillDownRequest
    {
        public DrillDownRequest(
            int custId,
            int viewLevel,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string groupName)
        {
            CustId = custId;
            ViewLevel = viewLevel;
            FromDate = fromDate;
            ToDate = toDate;
            ProductIds = productIds;
            GroupName = groupName;

            switch (ViewLevel)
            {
                case Role.Super:
                    SuperId = custId;
                    MasterId = 0;
                    AgentId = 0;
                    break;

                case Role.Master:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = custId;
                    AgentId = 0;
                    break;

                default:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = CheckRequest.GetIntRequest("mrecommend", 0);
                    AgentId = custId;
                    break;
            }
        }

        public int CustId { get; }

        public int SuperId { get; }

        public int MasterId { get; }

        public int AgentId { get; }

        public int ViewLevel { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }

        public string GroupName { get; }

        public bool IsValid => !ViewLevel.IsMember()
                               && ViewLevel <= Role.Group
                               && (ToDate >= FromDate)
                               && CustId > 0;

        public bool IsNotValid => !IsValid;
    }

    public class WinLossDetailRequestParam
    {
        public string groupName { get; set; }

        public int srecommend { get; set; }

        public int mrecommend { get; set; }

        public int recommend { get; set; }

        public int custId { get; set; }

        public string username { get; set; }

        public DateTime fromdate { get; set; }

        public DateTime todate { get; set; }

        public int viewlevel { get; set; }

        public bool IsSearch() => custId == 0;

    }
}