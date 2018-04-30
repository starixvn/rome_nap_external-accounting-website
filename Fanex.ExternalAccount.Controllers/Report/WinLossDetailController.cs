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

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseController
    {
        private readonly ICache cacheManager;
        private readonly WinLossDetailService winLossDetailService;
        private readonly dynamic ToolTipsProducts;

        public WinLossDetailController(
            ICache cacheManager,
            ICurrentSession currentSession,
            WinLossDetailService winLossDetailService)
        {
            this.cacheManager = cacheManager ?? CacheService.CreateInstance(CacheType.MemoryCache);
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();

            ToolTipsProducts = new
            {
                showAllbet = WinLossDetailHelper.IsShowProduct("showAB", currentSession ?? CurrentSession)
            };
        }

        public WinLossDetailController() : this(null, null, null)
        {
        }

        public ActionResult Index() => View();

        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
        {
            IWinLossDetailReportBuilder builder;

            switch (request.viewlevel)
            {
                case Role.Member:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetailAg>>(request.GetCacheKey());
                        if (winlossDetailData == null)
                        {
                            GetDataFromDB(request);
                            winlossDetailData = cacheManager.Get<List<WinLossDetailAg>>(request.GetCacheKey());
                        }

                        builder = new MemberWinLossDetailReportBuilder(
                            winlossDetailData,
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                case Role.Agent:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetailMAg>>(request.GetCacheKey());
                        if (winlossDetailData == null)
                        {
                            GetDataFromDB(request);
                            winlossDetailData = cacheManager.Get<List<WinLossDetailMAg>>(request.GetCacheKey());
                        }

                        builder = new AgentWinLossDetailReportBuilder(
                            winlossDetailData,
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                default:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetail>>(request.GetCacheKey());
                        if (winlossDetailData == null)
                        {
                            GetDataFromDB(request);
                            winlossDetailData = cacheManager.Get<List<WinLossDetail>>(request.GetCacheKey());
                        }

                        builder = new WinLossDetailReportBuilder(
                            winlossDetailData,
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
            }

            var formatedProducts = request.GetProductNames(GetProductList());
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, ToolTipsProducts))
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, ToolTipsProducts));
        }
    }

    public class WinLossDetailRequest
    {
        private const string Seperator = ", ";
        private const string ShowAll = "showAll";

        private readonly Func<ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter;
        private readonly ICurrentSession currentSession;

        public WinLossDetailRequest() : this(null, null)
        {
        }

        public WinLossDetailRequest(
            Func<ICollection<KeyValuePair<string, bool>>> selectedProductQueryGetter,
            ICurrentSession currentSession)
        {
            this.selectedProductQueryGetter = selectedProductQueryGetter ?? new FilterController().AsCollection;
            this.currentSession = currentSession ?? new Core.Context.CurrentSession();
        }

        public string groupName => currentSession.IsInternalAdmin
            ? CheckRequest.GetStringRequest("groupname", string.Empty)
            : currentSession.CurrencyGroup;

        public string username { get; set; }
        public int srecommend { get; set; }
        public int mrecommend { get; set; }
        public int recommend { get; set; }
        public int custId { get; set; }
        public DateTime fromdate { get; set; }
        public DateTime todate { get; set; }
        public int viewlevel { get; set; }

        public bool IsSearch() => custId == 0;

        internal string GetProductIds()
        {
            var filterParams = selectedProductQueryGetter();

            var formatedProductIds = new List<int>();

            foreach (ProductElementConfiguration item in ProductsConfiguration.Instance.ProductCollection)
            {
                foreach (var product in filterParams)
                {
                    if (item.ProductId != 0 && product.Key.Equals(item.Id, StringComparison.OrdinalIgnoreCase)
                        && product.Value)
                    {
                        formatedProductIds.Add(item.ProductId);
                    }
                }
            }

            return string.Join(",", formatedProductIds);
        }

        internal string GetProductNames(List<KeyValuePair<string, string>> productList)
        {
            var filterParams = selectedProductQueryGetter();
            var formatedProducts = string.Empty;

            filterParams.ToList().ForEach(o =>
            {
                productList.ForEach(p =>
                {
                    if (!p.Key.Equals(ShowAll, StringComparison.OrdinalIgnoreCase) && o.Value
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

        public string GetCacheKey()
        {
            string querySpecification = IsSearch() ? username : custId.ToString();

            return string.Join(":", "WinLossDetail", querySpecification, fromdate, todate, GetProductIds()).ToLowerInvariant();
        }
    }

    public class SearchWinLossDetailRequest : ISearchWinLossDetailRequest
    {
        public SearchWinLossDetailRequest(WinLossDetailRequest request, dynamic toolTipsProducts)
        {
            GroupName = request.groupName;
            Username = request.username;
            FromDate = request.fromdate;
            ToDate = request.todate;
            ProductIds = request.GetProductIds();
            ToolTipsProducts = toolTipsProducts;
            GetCacheKey = request.GetCacheKey;
        }

        public string GroupName { get; }
        public string Username { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }
        public dynamic ToolTipsProducts { get; }

        public Func<string> GetCacheKey { get; }
    }

    public class DrillDownWinLossDetailRequest : IDrillDownWinLossDetailRequest
    {
        public DrillDownWinLossDetailRequest(WinLossDetailRequest request, dynamic toolTipsProducts)
        {
            GroupName = request.groupName;
            CustId = request.custId;
            FromDate = request.fromdate;
            ToDate = request.todate;
            ViewLevel = request.viewlevel;
            ProductIds = request.GetProductIds();
            ToolTipsProducts = toolTipsProducts;

            GetCacheKey = request.GetCacheKey;

            switch (ViewLevel)
            {
                case Role.Super:
                    SuperId = CustId;
                    MasterId = 0;
                    AgentId = 0;
                    break;

                case Role.Master:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = CustId;
                    AgentId = 0;
                    break;

                default:
                    SuperId = CheckRequest.GetIntRequest("srecommend", 0);
                    MasterId = CheckRequest.GetIntRequest("mrecommend", 0);
                    AgentId = CustId;
                    break;
            }
        }

        public string GroupName { get; }
        public int SuperId { get; }
        public int MasterId { get; }
        public int AgentId { get; }
        public int CustId { get; }
        public int ViewLevel { get; }
        public DateTime FromDate { get; }
        public DateTime ToDate { get; }
        public string ProductIds { get; }
        public dynamic ToolTipsProducts { get; }

        public Func<string> GetCacheKey { get; }

        public bool IsValid => !ViewLevel.IsMember()
                               && ViewLevel <= Role.Group
                               && (ToDate >= FromDate)
                               && CustId > 0;

        public bool IsNotValid => !IsValid;
    }

    public class ExportWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}WinLossDetail_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}