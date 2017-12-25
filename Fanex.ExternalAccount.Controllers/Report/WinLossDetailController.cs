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
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class LineIds
    {
        public LineIds(int superId, int masterId, int agentId)
        {
            SuperId = superId;
            MasterId = masterId;
            AgentId = agentId;
        }

        public int SuperId { get; }

        public int MasterId { get; }

        public int AgentId { get; }
    }
    public class WinLossDetailController : BaseController
    {
        private const string MonthDayYearFormat = "MMM, dd yyyy";
        private const string Seperator = ", ";
        private readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IReportService _reportService = new ReportService();
        private string _cacheList = string.Empty;
        private int _custId;
        private string _formatedProducts = string.Empty;
        private DateTime _fromDate = DateTime.Today;
        private string _groupname = string.Empty;
        private int _memberId;
        private int _mrecommend;
        private string _productIds = string.Empty;
        private string _products = string.Empty;
        private int _recommend;
        private int _roleId = 5;
        private ArrayList _sortingOrderList;
        private int _srecommend;
        private DateTime _toDate = DateTime.Today;

        private string _username = string.Empty;
        private int _viewlevel = 5;

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
            _srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            _mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            _recommend = CheckRequest.GetIntRequest("recommend", 0);
            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;

            _sortingOrderList = _reportService.GetSortingOrderList(_groupname);

            if (_custId != 0)
            {
                return DrillDownWinLossDetail(
                    _custId,
                    _viewlevel,
                    _fromDate,
                    _toDate,
                    _productIds,
                    _sortingOrderList);
            }

            return SearchWinLossDetail(
                _username,
                _groupname,
                _fromDate,
                _toDate,
                _productIds,
                _sortingOrderList);
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

        private bool SearchCustomer(string userName, string groupname)
        {
            if (!string.IsNullOrWhiteSpace(userName))
            {
                var custInfo = _reportService.GetCustomerInfo(groupname, 0, userName, true);
                if (custInfo == null || custInfo.Rows.Count == 0)
                {
                    return false;
                }

                _roleId = custInfo.Rows[0].ToInt32("roleid");
                _srecommend = custInfo.Rows[0].ToInt32("srecommend");
                _mrecommend = custInfo.Rows[0].ToInt32("mrecommend");
                _recommend = custInfo.Rows[0].ToInt32("recommend");
                _memberId = custInfo.Rows[0].ToInt32("custid");
            }

            return true;
        }

        private ActionResult SearchWinLossDetail(
            string username,
            string groupname,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            ArrayList sortingOrderList)
        {
            if (!SearchCustomer(username, groupname))
            {
                return JsonBase(
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

            if (_roleId >= Role.Super)
            {
                var _listWinLossDetail = _reportService.GetWinLossDetail(groupname, _srecommend, fromDate, toDate, productIds);

                _cache.Set(_cacheList, _listWinLossDetail);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Super,
                        supername = username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = _listWinLossDetail,
                        sortingorderlist = sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }

            if (_roleId == Role.Master)
            {
                var _listWinLossDetail = _reportService.GetWinLossDetailSuper(
                    _srecommend, _mrecommend, fromDate, toDate, productIds);
                _cache.Set(_cacheList, _listWinLossDetail);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Master,
                        supername = string.Empty,
                        mastername = username,
                        agentname = string.Empty,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        rows = _listWinLossDetail,
                        sortingorderlist = sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId == Role.Agent)
            {
                var _listWinLossDetailMAg = _reportService.GetWinLossDetailMAg(
                    _mrecommend, _recommend, fromDate, toDate, productIds);
                _cache.Set(_cacheList, _listWinLossDetailMAg);
                return JsonBase(
                    new
                    {
                        viewlevel = Role.Agent,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = username,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        recommend = _recommend,
                        rows = _listWinLossDetailMAg,
                        sortingorderlist = sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            var _listWinLossDetailAg = _reportService.GetWinLossDetailAg(
                _recommend, _memberId, fromDate, toDate, productIds);
            _cache.Set(_cacheList, _listWinLossDetailAg);
            return JsonBase(
                new
                {
                    viewlevel = Role.Member,
                    supername = string.Empty,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _recommend,
                    rows = _listWinLossDetailAg,
                    sortingorderlist = sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }

        private ActionResult DrillDownWinLossDetail(
            int custId,
            int viewlevel,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            ArrayList sortingOrderList)
        {
            if (viewlevel == Role.Super)
            {
                return DrillDownSuperWinLossDetail(custId, fromDate, toDate, productIds, viewlevel.DirectDownlineLevel(), sortingOrderList);
            }

            if (viewlevel == Role.Master)
            {
                return DrillDownMasterWinLossDetail(custId, fromDate, toDate, productIds, viewlevel.DirectDownlineLevel(), sortingOrderList);
            }

            return DrillDownAgentWinLossDetail(custId, fromDate, toDate, productIds, viewlevel.DirectDownlineLevel(), sortingOrderList);
        }

        private ActionResult DrillDownSuperWinLossDetail(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            int directDownlineLevel,
            ArrayList sortingOrderList)
        {
            var winLossData = _reportService.GetWinLossDetailSuper(custId, 0, fromDate, toDate, productIds);

            _cache.Set(_cacheList, winLossData);

            return JsonBase(
                new
                {
                    viewlevel = directDownlineLevel,
                    srecommend = custId,
                    mrecommend = 0,
                    recommend = 0,
                    rows = winLossData,
                    sortingorderlist = sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }

        private ActionResult DrillDownMasterWinLossDetail(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            int directDownlineLevel,
            ArrayList sortingOrderList)
        {
            var winLossData = _reportService.GetWinLossDetailMAg(custId, 0, fromDate, toDate, productIds);
            _cache.Set(_cacheList, winLossData);

            return JsonBase(
                new
                {
                    viewlevel = directDownlineLevel,
                    srecommend = _srecommend,
                    mrecommend = custId,
                    recommend = 0,
                    rows = winLossData,
                    sortingorderlist = sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }

        private ActionResult DrillDownAgentWinLossDetail(
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            int directDownlineLevel,
            ArrayList sortingOrderList)
        {
            var winLossData = _reportService.GetWinLossDetailAg(custId, 0, fromDate, toDate, productIds);

            _cache.Set(_cacheList, winLossData);

            return JsonBase(
                new
                {
                    viewlevel = directDownlineLevel,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = custId,
                    rows = winLossData,
                    sortingorderlist = sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }
    }
}