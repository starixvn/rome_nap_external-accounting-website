namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Http;
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
        public ActionResult GetData([FromUri]WinLossDetailRequestParam request)
        {
            var viewlevel = request.viewlevel;
            var custId = request.custId;
            var fromDate = request.fromdate;
            var username = request.username;
            var toDate = request.todate;
            var productIds = GetProductIds(new FilterController().AsCollection());

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
        public void ExportWinLossDetailToExcel([FromUri]WinLossDetailRequestParam request)
        {
            var productCollection = new FilterController().AsCollection();
            var fromDate = request.fromdate;
            var dateTime = request.todate;
            var formatedProducts = GetProductNames(productCollection);
            var viewlevel = request.viewlevel;
            var productIds = GetProductIds(productCollection);
            var custId = request.custId;

            var cacheKey = "CacheList" + request.username + GroupName + custId + fromDate + dateTime + productIds;

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
                            dateTime);

                        break;
                    }
                case Role.Agent:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetailMAg>>(cacheKey);
                        builder = new AgentWinLossDetailReportBuilder(
                            winlossDetailData,
                            viewlevel,
                            fromDate,
                            dateTime);

                        break;
                    }
                default:
                    {
                        var winlossDetailData = cacheManager.Get<List<WinLossDetail>>(cacheKey);
                        builder = new WinLossDetailReportBuilder(
                            winlossDetailData,
                            viewlevel,
                            fromDate,
                            dateTime);

                        break;
                    }
            }

            var memoryStream = builder.Build(formatedProducts);

            var exportFileName =
                $"{viewlevel.DisplayRoleName()}WinLossDetail_{fromDate.ToString(DateFormat)}_{dateTime.ToString(DateFormat)}.xls";

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, exportFileName);
                memoryStream.Dispose();
            }
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