namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
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
            ArrayList sortingOrderList)
        {
            CustId = custId;
            ViewLevel = viewLevel;
            FromDate = fromDate;
            ToDate = toDate;
            ProductIds = productIds;
            SortingOrderList = sortingOrderList;

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
        public ArrayList SortingOrderList { get; }

        public bool IsValid => !ViewLevel.IsMember()
            && ViewLevel <= Role.Group
            && (ToDate >= FromDate)
            && CustId > 0;

        public bool IsNotValid => !IsValid;
    }

    public class WinLossDetailController : BaseController
    {
        private const string Seperator = ", ";
        private const string DateFormat = "yyyyMMdd";

        private readonly ICache _cache;
        private readonly IReportService _reportService;

        private readonly Dictionary<int, Func<int, int, DateTime, DateTime, string, object>>
            GetWinLossDetailFunctions;

        private readonly object ToolTipsProducts;

        private string _formatedProducts = string.Empty;
        private string _productIds = string.Empty;
        private string _products = string.Empty;
        private string _groupname = string.Empty;

        private int _custId;
        private int _viewlevel = 5;

        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;
        private string _cacheList = string.Empty;

        public WinLossDetailController(
            IReportService reportService,
            ICache cacheManager,
            ICurrentSession currentSession)
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
        public ActionResult Index() => PartialViewBase();

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

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail, Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetData(int viewLevel = 0)
        {
            GetRequestPage();
            _viewlevel = viewLevel != 0 ? viewLevel : _viewlevel;

            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;

            var sortingOrderList = _reportService.GetSortingOrderList(_groupname);

            if (_custId != 0)
            {
                return DrillDownWinLossDetail(new DrillDownRequest(
                    _custId,
                    _viewlevel,
                    _fromDate,
                    _toDate,
                    _productIds,
                    sortingOrderList));
            }

            return SearchWinLossDetail(
                _username,
                _groupname,
                _fromDate,
                _toDate,
                _productIds,
                sortingOrderList);
        }

        private ActionResult SearchWinLossDetail(
            string username,
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            ArrayList sortingOrderList)
        {
            var customerInfo = new CustomerInfoSearchResult { RoleId = Role.Super };

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
                var getWinLossDetail = GetWinLossDetailFunctions[customerInfo.RoleId.DirectUplineLevel()];

                winLossDetailData = getWinLossDetail(
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
                    tooltips = ToolTipsProducts
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
                    tooltips = ToolTipsProducts
                });
        }

        private ActionResult DrillDownWinLossDetail(DrillDownRequest request)
        {
            if (request.IsNotValid)
            {
                throw new InvalidOperationException(nameof(request.IsValid));
            }

            var getWinLossDetail = GetWinLossDetailFunctions[request.ViewLevel];

            var winLossData = getWinLossDetail(
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
                    tooltips = ToolTipsProducts
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
          Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public void ExportWinLossDetailToExcel()
        {
            GetRequestPage();
            var fromDate = _fromDate;
            var dateTime = _toDate;
            var formatedProducts = _formatedProducts;

            var getNew = false;
            _cacheList = "CacheList" + _username + _groupname + _custId + fromDate + dateTime + _products;
            var level = _viewlevel;

            string fileName;
            IWinLossDetailReportBuilder builder;

            switch (_viewlevel)
            {
                case Role.Member:
                    {
                        if (!Config.IsCache || _cache.Get<List<WinLossDetailAg>>(_cacheList) == null)
                        {
                            getNew = true;
                            GetData(Role.Agent);
                        }

                        level = !getNew ? _viewlevel : Role.Member;

                        builder = new WinLossDetailReportAgBuilder(
                            _cache.Get<List<WinLossDetailAg>>(_cacheList),
                            level,
                            fromDate,
                            dateTime);

                        fileName = level == Role.Agent ? "Agent" : "Member";

                        break;
                    }
                case Role.Agent:
                    {
                        if (!Config.IsCache || _cache.Get<List<WinLossDetailMAg>>(_cacheList) == null)
                        {
                            getNew = true;
                            GetData(Role.Master);
                        }

                        level = !getNew ? _viewlevel : Role.Agent;

                        builder = new WinLossDetailReportMAgBuilder(
                            _cache.Get<List<WinLossDetailMAg>>(_cacheList),
                            level,
                            fromDate,
                            dateTime);

                        fileName = level == Role.Master ? "Master" : "Agent";

                        break;
                    }
                default:
                    {
                        if (!Config.IsCache || _cache.Get<List<WinLossDetail>>(_cacheList) == null)
                        {
                            getNew = true;
                            GetData(Role.Super);
                        }

                        level = level == Role.Super ? level : !getNew ? _viewlevel : Role.Master;

                        builder = new WinLossDetailReportBuilder(
                            _cache.Get<List<WinLossDetail>>(_cacheList),
                            level,
                            fromDate,
                            dateTime);

                        fileName = level == Role.Super ? "Super" : "Master";

                        break;
                    }
            }

            var memoryStream = builder.Build(formatedProducts);

            var exportFileName =
                $"{fileName}WinLossDetail_{fromDate.ToString(DateFormat)}_{dateTime.ToString(DateFormat)}.xls";

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, exportFileName);
                memoryStream.Dispose();
            }
        }
    }
}