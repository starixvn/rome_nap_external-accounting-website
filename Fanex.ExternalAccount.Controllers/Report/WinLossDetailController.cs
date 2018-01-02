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

        private readonly ICache _cache;
        private readonly WinLossDetailService winLossDetailService;

        private readonly object ToolTipsProducts;

        private string _formatedProducts = string.Empty;
        private string _productIds = string.Empty;
        private string _products = string.Empty;
        private string _groupname = string.Empty;

        private int _custId;
        private int _viewlevel = Role.Group;

        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;

        public WinLossDetailController(
            ICache cacheManager,
            ICurrentSession currentSession,
            WinLossDetailService winLossDetailService)
        {
            _cache = cacheManager ?? CacheService.CreateInstance(CacheType.MemoryCache);
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

        public void GetRequestPage()
        {
            _products = string.Empty;
            _formatedProducts = string.Empty;
            var _formatedProductIds = new List<int>();

            _username = CheckRequest.GetStringRequest("username", string.Empty);
            _viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            _custId = CheckRequest.GetIntRequest("custid", 0);
            _groupname = CurrentSession.IsInternalAdmin
                ? CheckRequest.GetStringRequest("groupname", string.Empty)
                : CurrentSession.CurrencyGroup;
            _fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            _toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);

            var filterParams = new FilterController().AsCollection();
            var list = GetProductList();
            filterParams.ToList().ForEach(o =>
            {
                _products += o.Key + o.Value;
                list.ForEach(p =>
                {
                    if (!p.Key.Equals("showAll", StringComparison.OrdinalIgnoreCase) && o.Value
                        && o.Key.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        _formatedProducts += p.Value + Seperator;
                    }
                });
            });

            if (_formatedProducts.Length > 0)
            {
                _formatedProducts = _formatedProducts.TrimEnd(Seperator.ToCharArray());
            }

            foreach (ProductElementConfiguration item in ProductsConfiguration.Instance.ProductCollection)
            {
                foreach (var product in filterParams)
                {
                    if (item.ProductId != 0 && product.Key.Equals(item.Id, StringComparison.OrdinalIgnoreCase) && product.Value)
                    {
                        _formatedProductIds.Add(item.ProductId);
                    }
                }
            }

            _productIds = string.Join(",", _formatedProductIds);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetData(int viewLevel = 0)
        {
            GetRequestPage();
            _viewlevel = viewLevel != 0 ? viewLevel : _viewlevel;
            var isSearchAction = _custId == 0;

            var viewModel =
                isSearchAction
                ? winLossDetailService.SearchWinLossDetail(_username, _groupname, _fromDate, _toDate, _productIds, ToolTipsProducts)
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownRequest(_custId, _viewlevel, _fromDate, _toDate, _productIds, _groupname),
                    ToolTipsProducts);

            return Json(viewModel);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
          Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public void ExportWinLossDetailToExcel()
        {
            GetRequestPage();
            var fromDate = _fromDate;
            var dateTime = _toDate;
            var formatedProducts = _formatedProducts;
            var roleId = _viewlevel;
            var cacheKey = "CacheList" + _username + _groupname + _custId + fromDate + dateTime + _productIds;

            IWinLossDetailReportBuilder builder;

            switch (roleId)
            {
                case Role.Member:
                    {
                        var winlossDetailData = _cache.Get<List<WinLossDetailAg>>(cacheKey);
                        builder = new MemberWinLossDetailReportBuilder(
                            winlossDetailData,
                            roleId,
                            fromDate,
                            dateTime);

                        break;
                    }
                case Role.Agent:
                    {
                        var winlossDetailData = _cache.Get<List<WinLossDetailMAg>>(cacheKey);
                        builder = new AgentWinLossDetailReportBuilder(
                            winlossDetailData,
                            roleId,
                            fromDate,
                            dateTime);

                        break;
                    }
                default:
                    {
                        var winlossDetailData = _cache.Get<List<WinLossDetail>>(cacheKey);
                        builder = new WinLossDetailReportBuilder(
                            winlossDetailData,
                            roleId,
                            fromDate,
                            dateTime);

                        break;
                    }
            }

            var memoryStream = builder.Build(formatedProducts);

            var exportFileName =
                $"{roleId.DisplayRoleName()}WinLossDetail_{fromDate.ToString(DateFormat)}_{dateTime.ToString(DateFormat)}.xls";

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
}