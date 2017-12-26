namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Caching;
    using Core.Base;
    using Core.Context;
    using Core.Reports;
    using Core.Reports.WinLossDetail;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Customer;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class DrillDownRequest
    {
        public DrillDownRequest(
            int custId,
            int viewLevel,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            ArrayList sortingOrderList,
            object toolTipsProducts)
        {
            CustId = custId;
            ViewLevel = viewLevel;
            FromDate = fromDate;
            ToDate = toDate;
            ProductIds = productIds;
            SortingOrderList = sortingOrderList;
            ToolTipsProducts = toolTipsProducts;

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

        public int CustId { get; private set; }

        public int SuperId { get; }

        public int MasterId { get; }

        public int AgentId { get; }

        public int ViewLevel { get; private set; }
        public DateTime FromDate { get; private set; }
        public DateTime ToDate { get; private set; }
        public string ProductIds { get; private set; }
        public ArrayList SortingOrderList { get; private set; }
        public object ToolTipsProducts { get; private set; }

        public bool IsValid => !ViewLevel.IsMember()
            && ViewLevel <= Role.Group
            && (ToDate >= FromDate)
            && CustId > 0;

        public bool IsNotValid => !IsValid;
    }

    public class WinLossDetailController : BaseController
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";
        private const string Seperator = ", ";

        private readonly ICache _cache;
        private readonly IReportService _reportService;

        private readonly Dictionary<int, Func<int, int, DateTime, DateTime, string, object>>
            GetWinLossDetailFunctions;

        private string _formatedProducts = string.Empty;
        private string _productIds = string.Empty;
        private string _products = string.Empty;
        private string _groupname = string.Empty;
        private int _custId;
        private int _roleId = 5;
        private int _viewlevel = 5;

        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;

        private string _cacheList = string.Empty;

        public WinLossDetailController(IReportService reportService, ICache cacheManager)
        {
            _reportService = reportService ?? new ReportService();
            _cache = cacheManager ?? CacheService.CreateInstance(CacheType.MemoryCache);

            GetWinLossDetailFunctions =
                new Dictionary<int, Func<int, int, DateTime, DateTime, string, object>>
                {
                    {Role.Super, _reportService.GetWinLossDetailSuper},
                    {Role.Master, _reportService.GetWinLossDetailMAg},
                    {Role.Agent, _reportService.GetWinLossDetailAg}
                };
        }

        public WinLossDetailController() : this(null, null)
        {
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index() => PartialViewBase();

        public void GetRequestPage()
        {
            _products = string.Empty;
            _formatedProducts = string.Empty;
            var _formatedProductIds = new List<int>();
            _productIds = string.Empty;
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

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetData(int viewLevel = 0)
        {
            GetRequestPage();
            _viewlevel = viewLevel != 0 ? viewLevel : _viewlevel;

            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;

            var sortingOrderList = _reportService.GetSortingOrderList(_groupname);

            var toolTipsProducts = new
            {
                showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
            };

            if (_custId != 0)
            {
                return DrillDownWinLossDetail(new DrillDownRequest(
                    _custId,
                    _viewlevel,
                    _fromDate,
                    _toDate,
                    _productIds,
                    sortingOrderList,
                    toolTipsProducts));
            }

            return SearchWinLossDetail(
                _username,
                _groupname,
                _fromDate,
                _toDate,
                _productIds,
                sortingOrderList,
                toolTipsProducts);
        }

        private ActionResult SearchWinLossDetail(
            string username,
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            ArrayList sortingOrderList,
            object toolTipsProducts)
        {
            var customerInfo = new CustomerInfoSearchResult { RoleId = _roleId };

            if (!string.IsNullOrWhiteSpace(username))
            {
                customerInfo = _reportService.SearchCustomerInfo(groupname, 0, username, true);

                if (customerInfo == null)
                {
                    return NoInfoResult(username, sortingOrderList);
                }
            }

            object winLossDetailData;

            if (customerInfo.RoleId >= Role.Super)
            {
                winLossDetailData = _reportService.GetWinLossDetail(
                    groupname,
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }
            else
            {
                var getWinLossDetailFunction = GetWinLossDetailFunctions[customerInfo.RoleId.DirectUplineLevel()];

                winLossDetailData = getWinLossDetailFunction(
                    customerInfo.GetCustomerUplineId(),
                    customerInfo.CustId,
                    fromDate,
                    toDate,
                    productIds);
            }

            _cache.Set(_cacheList, winLossDetailData);

            return Json(
                new
                {
                    viewlevel = customerInfo.RoleId,
                    supername = customerInfo.SuperName,
                    mastername = customerInfo.MasterName,
                    agentname = customerInfo.AgentName,
                    membername = customerInfo.MemberName,
                    srecommend = customerInfo.SuperId,
                    mrecommend = customerInfo.MasterId,
                    recommend = customerInfo.AgentId,
                    rows = winLossDetailData,
                    sortingorderlist = sortingOrderList,
                    tooltips = toolTipsProducts
                });
        }

        private ActionResult NoInfoResult(string username, ArrayList sortingOrderList)
        {
            return Json(
                new
                {
                    viewlevel = Role.Super,
                    supername = username,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    rows = string.Empty,
                    sortingorderlist = sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                });
        }

        private ActionResult DrillDownWinLossDetail(DrillDownRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }

            var getWinLossDetailByLevel = GetWinLossDetailFunctions[request.ViewLevel];

            var winLossData = getWinLossDetailByLevel(
                request.CustId,
                0,
                request.FromDate,
                request.ToDate,
                request.ProductIds);

            // TODO: will be removed later
            _cache.Set(_cacheList, winLossData);

            return Json(
                new
                {
                    viewlevel = request.ViewLevel.DirectDownlineLevel(),
                    srecommend = request.SuperId,
                    mrecommend = request.MasterId,
                    recommend = request.AgentId,
                    rows = winLossData,
                    sortingorderlist = request.SortingOrderList,
                    tooltips = request.ToolTipsProducts
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
          Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public void ExportWinLossDetailToExcel()
        {
            GetRequestPage();
            var subFileName = _fromDate.ToString("yyyyMMdd") + "_" + _toDate.ToString("yyyyMMdd") + ".xls";
            var getNew = false;
            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;
            var level = _viewlevel;

            if (_viewlevel == Role.Member)
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetailAg>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(Role.Agent);
                }

                level = !getNew ? _viewlevel : Role.Member;
                var builder = new WinLossDetailReportAgBuilder(
                    _cache.Get<List<WinLossDetailAg>>(_cacheList),
                    false,
                    !getNew ? _viewlevel : Role.Member,
                    _fromDate.ToString(MonthDayYearFormat),
                    _toDate.ToString(MonthDayYearFormat));
                var excelStream = builder.Build(_formatedProducts);
                if (excelStream != null)
                {
                    ExportToExcel(
                        excelStream,
                        level == Role.Agent ? "AgentWinLossDetail_" + subFileName : "MemberWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
            else if (_viewlevel == 2)
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetailMAg>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(Role.Master);
                }

                level = !getNew ? _viewlevel : Role.Agent;
                var builder = new WinLossDetailReportMAgBuilder(
                    _cache.Get<List<WinLossDetailMAg>>(_cacheList),
                    false,
                    level,
                    _fromDate.ToString(MonthDayYearFormat),
                    _toDate.ToString(MonthDayYearFormat));
                var excelStream = builder.Build(_formatedProducts);
                if (excelStream != null)
                {
                    ExportToExcel(
                        excelStream,
                        level == Role.Master ? "MasterWinLossDetail_" + subFileName : "AgentWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
            else
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetail>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(Role.Super);
                }

                level = level == Role.Super ? level : !getNew ? _viewlevel : Role.Master;

                var builder = new WinLossDetailReportBuilder(
                    _cache.Get<List<WinLossDetail>>(_cacheList),
                    false,
                    level,
                    _fromDate,
                    _toDate);
                var excelStream = builder.Build(_formatedProducts);
                if (excelStream != null)
                {
                    ExportToExcel(
                        excelStream,
                        level == Role.Super ? "SuperWinLossDetail_" + subFileName : "MasterWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
        }
    }
}