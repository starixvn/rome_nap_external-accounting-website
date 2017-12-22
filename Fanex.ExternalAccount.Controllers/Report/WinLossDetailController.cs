namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Caching;
    using Core.Base;
    using Core.Context;
    using Core.Reports;
    using Core.Reports.WinLossDetail;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class WinLossDetailController : BaseController
    {
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
                        _formatedProducts += p.Value + ", ";
                    }
                });
            });

            if (_formatedProducts.Length > 0)
            {
                _formatedProducts = _formatedProducts.Substring(0, _formatedProducts.Length - 2);
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
            _srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            _mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            _recommend = CheckRequest.GetIntRequest("recommend", 0);
            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;

            _sortingOrderList = _reportService.GetSortingOrderList(_groupname);

            if (_custId != 0)
            {
                return GetWinLossDetailDownline();
            }

            return GetWinLossDetailSearch();
        }


        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public void GetExportToExcelWinLossDetail()
        {
            GetRequestPage();
            var subFileName = _fromDate.ToString("yyyyMMdd") + "_" + _toDate.ToString("yyyyMMdd") + ".xls";
            var getNew = false;
            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate + _products;
            var level = _viewlevel;

            if (_viewlevel == 1)
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetailAg>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(2);
                }

                level = !getNew ? _viewlevel : 1;
                var builder = new WinLossDetailReportAgBuilder(
                    _cache.Get<List<WinLossDetailAg>>(_cacheList),
                    false,
                    !getNew ? _viewlevel : 1,
                    _fromDate.ToString("MMM, dd yyyy"),
                    _toDate.ToString("MMM, dd yyyy"));
                var excelStream = builder.Build(_formatedProducts);
                if (excelStream != null)
                {
                    ExportToExcel(
                        excelStream,
                        level == 2 ? "AgentWinLossDetail_" + subFileName : "MemberWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
            else if (_viewlevel == 2)
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetailMAg>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(3);
                }

                level = !getNew ? _viewlevel : 2;
                var builder = new WinLossDetailReportMAgBuilder(
                    _cache.Get<List<WinLossDetailMAg>>(_cacheList),
                    false,
                    level,
                    _fromDate.ToString("MMM, dd yyyy"),
                    _toDate.ToString("MMM, dd yyyy"));
                var excelStream = builder.Build(_formatedProducts);
                if (excelStream != null)
                {
                    ExportToExcel(
                        excelStream,
                        level == 3 ? "MasterWinLossDetail_" + subFileName : "AgentWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
            else
            {
                if (!Config.IsCache || _cache.Get<List<WinLossDetail>>(_cacheList) == null)
                {
                    getNew = true;
                    GetData(4);
                }

                level = level == 4 ? level : !getNew ? _viewlevel : 3;

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
                        level == 4 ? "SuperWinLossDetail_" + subFileName : "MasterWinLossDetail_" + subFileName);
                    excelStream.Dispose();
                }
            }
        }

        private bool CheckSearchCustomer()
        {
            if (!string.IsNullOrWhiteSpace(_username))
            {
                var custInfo = _reportService.GetCustomerInfo(_groupname, 0, _username, true);
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
        
        private ActionResult GetWinLossDetailSearch()
        {
            if (!CheckSearchCustomer())
            {
                return JsonBase(
                    new
                    {
                        viewlevel = 4,
                        supername = _username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = string.Empty,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    });
            }

            if (_roleId > 3)
            {
                var _listWinLossDetail = _reportService.GetWinLossDetail(
                    _groupname, _srecommend, _fromDate, _toDate, _productIds);
                _cache.Set(_cacheList, _listWinLossDetail);
                return JsonBase(
                    new
                    {
                        viewlevel = 4,
                        supername = _username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = _listWinLossDetail,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId == 3)
            {
                var _listWinLossDetail = _reportService.GetWinLossDetailSuper(
                    _srecommend, _mrecommend, _fromDate, _toDate, _productIds);
                _cache.Set(_cacheList, _listWinLossDetail);
                return JsonBase(
                    new
                    {
                        viewlevel = 3,
                        supername = string.Empty,
                        mastername = _username,
                        agentname = string.Empty,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        rows = _listWinLossDetail,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            if (_roleId == 2)
            {
                var _listWinLossDetailMAg = _reportService.GetWinLossDetailMAg(
                    _mrecommend, _recommend, _fromDate, _toDate, _productIds);
                _cache.Set(_cacheList, _listWinLossDetailMAg);
                return JsonBase(
                    new
                    {
                        viewlevel = 2,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = _username,
                        srecommend = _srecommend,
                        mrecommend = _mrecommend,
                        recommend = _recommend,
                        rows = _listWinLossDetailMAg,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            var _listWinLossDetailAg = _reportService.GetWinLossDetailAg(
                _recommend, _memberId, _fromDate, _toDate, _productIds);
            _cache.Set(_cacheList, _listWinLossDetailAg);
            return JsonBase(
                new
                {
                    viewlevel = 1,
                    supername = string.Empty,
                    mastername = string.Empty,
                    agentname = string.Empty,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _recommend,
                    rows = _listWinLossDetailAg,
                    sortingorderlist = _sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }

        private ActionResult GetWinLossDetailDownline()
        {
            if (_viewlevel == 4)
            {
                var _listWinLossDetail = _reportService.GetWinLossDetailSuper(_custId, 0, _fromDate, _toDate, _productIds);
                _cache.Set(_cacheList, _listWinLossDetail);
                return JsonBase(
                    new
                    {
                        viewlevel = 3,
                        srecommend = _custId,
                        rows = _listWinLossDetail,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            if (_viewlevel == 3)
            {
                var _listWinLossDetailMAg = _reportService.GetWinLossDetailMAg(_custId, 0, _fromDate, _toDate, _productIds);
                _cache.Set(_cacheList, _listWinLossDetailMAg);
                return JsonBase(
                    new
                    {
                        viewlevel = 2,
                        srecommend = _srecommend,
                        mrecommend = _custId,
                        rows = _listWinLossDetailMAg,
                        sortingorderlist = _sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new JsonSerializerSettings());
            }
            var _listWinLossDetailAg = _reportService.GetWinLossDetailAg(_custId, 0, _fromDate, _toDate, _productIds);
            _cache.Set(_cacheList, _listWinLossDetailAg);
            return JsonBase(
                new
                {
                    viewlevel = 1,
                    srecommend = _srecommend,
                    mrecommend = _mrecommend,
                    recommend = _custId,
                    rows = _listWinLossDetailAg,
                    sortingorderlist = _sortingOrderList,
                    tooltips = new
                    {
                        showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                    }
                },
                new JsonSerializerSettings());
        }
    }
}