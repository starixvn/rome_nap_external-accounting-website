namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Reports;
    using Core.Reports.WinLossDetail;
    using Fanex.Caching;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    public class WinLossDetailController : BaseController
    {
        #region Init WinLossDetail

        private readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IReportService _reportService = new ReportService();
        private List<WinLossDetail> _listWinLossDetail = null;
        private List<WinLossDetailMAg> _listWinLossDetailMAg = null;
        private List<WinLossDetailAg> _listWinLossDetailAg = null;
        private ArrayList _sortingOrderList = null;
        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;
        private string _groupname = string.Empty;
        private string _cacheList = string.Empty;
        private int _roleId = 5;
        private int _viewlevel = 5;
        private int _srecommend = 0;
        private int _mrecommend = 0;
        private int _recommend = 0;
        private int _memberId = 0;
        private int _custId = 0;
        private readonly string _searchName = string.Empty;
        private ICollection<KeyValuePair<string, bool>> _filterParams = null;
        private readonly int _pageSize = 50;
        private int _totalPages = 1;
        private string _products = string.Empty;
        private string _formatedProducts = string.Empty;
        private IList<int> _formatedProductIds = new List<int>();
        private string _productIds = string.Empty;

        #endregion Init WinLossDetail

        [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index()
        {
            return this.PartialViewBase();
        }

        public void GetRequestPage()
        {
            this._products = string.Empty;
            this._formatedProducts = string.Empty;
            this._formatedProductIds = new List<int>();
            this._productIds = string.Empty;
            this._username = CheckRequest.GetStringRequest("username", string.Empty);
            this._viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            this._custId = CheckRequest.GetIntRequest("custid", 0);
            this._groupname = CurrentSession.IsInternalAdmin ? CheckRequest.GetStringRequest("groupname", string.Empty) : CurrentSession.CurrencyGroup;
            this._fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            this._toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
            this._filterParams = new FilterController().AsCollection();
            var list = base.GetProductList();
            this._filterParams.ToList().ForEach(o =>
            {
                this._products += o.Key + o.Value;
                list.ForEach(p =>
                {
                    if (!p.Key.Equals("showAll", StringComparison.OrdinalIgnoreCase) && o.Value && o.Key.Equals(p.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        this._formatedProducts += p.Value + ", ";
                    }
                });
            });

            if (this._formatedProducts.Length > 0)
            {
                this._formatedProducts = this._formatedProducts.Substring(0, this._formatedProducts.Length - 2);
            }

            var products = ProductsConfiguration.Instance.ProductCollection;
            foreach (ProductElementConfiguration item in products)
            {                
                foreach (var product in _filterParams)
                {
                    if (item.ProductId == 0)
                    {
                        break;
                    }

                    if (product.Key.Equals(item.Id, StringComparison.OrdinalIgnoreCase) && product.Value)
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
            this.GetRequestPage();
            this._viewlevel = viewLevel != 0 ? viewLevel : this._viewlevel;
            this._srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            this._mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            this._recommend = CheckRequest.GetIntRequest("recommend", 0);
            this._cacheList = "CacheList" + this._username + this._groupname + this._custId + this._fromDate.ToString() + this._toDate.ToString() + this._products;

            if (this.IsBelong(this._groupname, this._custId, null, true))
            {
                this._sortingOrderList = this._reportService.GetSortingOrderList(this._groupname);

                if (this._custId != 0)
                {
                    return this.GetWinLossDetailDownline();
                }
                else
                {
                    return this.GetWinLossDetailSearch();
                }
            }
            else
            {
                return this.JsonBase(
                    new
                    {
                        viewlevel = 5,
                        srecommend = this._srecommend,
                        mrecommend = this._mrecommend,
                        recommend = this._recommend,
                        rows = string.Empty,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        public void GetExportToExcelWinLossDetail()
        {
            this.GetRequestPage();
            string subFileName = this._fromDate.ToString("yyyyMMdd") + "_" + this._toDate.ToString("yyyyMMdd") + ".xls";
            bool getNew = false;
            this._cacheList = "CacheList" + this._username + this._groupname + this._custId + this._fromDate.ToString() + this._toDate.ToString() + this._products;
            int level = this._viewlevel;

            if (this.IsBelong(this._groupname, this._custId, null, true))
            {
                if (this._viewlevel == 1)
                {
                    if (!Config.IsCache || this._cache.Get<List<WinLossDetailAg>>(this._cacheList) == null)
                    {
                        getNew = true;
                        this.GetData(2);
                    }

                    level = !getNew ? this._viewlevel : 1;
                    var builder = new WinLossDetailReportAgBuilder(
                        this._cache.Get<List<WinLossDetailAg>>(this._cacheList),
                        false,
                        !getNew ? this._viewlevel : 1,
                        this._fromDate.ToString("MMM, dd yyyy"),
                        this._toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build(this._formatedProducts);
                    if (excelStream != null)
                    {
                        this.ExportToExcel(
                            excelStream, level == 2 ? "AgentWinLossDetail_" + subFileName : "MemberWinLossDetail_" + subFileName);
                        excelStream.Dispose();
                    }
                }
                else if (this._viewlevel == 2)
                {
                    if (!Config.IsCache || this._cache.Get<List<WinLossDetailMAg>>(this._cacheList) == null)
                    {
                        getNew = true;
                        this.GetData(3);
                    }

                    level = !getNew ? this._viewlevel : 2;
                    var builder = new WinLossDetailReportMAgBuilder(
                        this._cache.Get<List<WinLossDetailMAg>>(this._cacheList),
                        false,
                        level,
                        this._fromDate.ToString("MMM, dd yyyy"),
                        this._toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build(this._formatedProducts);
                    if (excelStream != null)
                    {
                        this.ExportToExcel(
                            excelStream, level == 3 ? "MasterWinLossDetail_" + subFileName : "AgentWinLossDetail_" + subFileName);
                        excelStream.Dispose();
                    }
                }
                else
                {
                    if (!Config.IsCache || this._cache.Get<List<WinLossDetail>>(this._cacheList) == null)
                    {
                        getNew = true;
                        this.GetData(4);
                    }

                    level = level == 4 ? level : !getNew ? this._viewlevel : 3;

                    var builder = new WinLossDetailReportBuilder(
                        this._cache.Get<List<WinLossDetail>>(this._cacheList),
                        false,
                        level,
                        this._fromDate.ToString("MMM, dd yyyy"),
                        this._toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build(this._formatedProducts);
                    if (excelStream != null)
                    {
                        this.ExportToExcel(
                            excelStream, level == 4 ? "SuperWinLossDetail_" + subFileName : "MasterWinLossDetail_" + subFileName);
                        excelStream.Dispose();
                    }
                }
            }
        }

        private bool CheckSearchCustomer()
        {
            if (!string.IsNullOrWhiteSpace(this._username))
            {
                var custInfo = this._reportService.GetCustomerInfo(this._groupname, 0, this._username, true);
                if (custInfo == null || custInfo.Rows.Count == 0)
                {
                    return false;
                }

                this._roleId = custInfo.Rows[0].ToInt32("roleid");
                this._srecommend = custInfo.Rows[0].ToInt32("srecommend");
                this._mrecommend = custInfo.Rows[0].ToInt32("mrecommend");
                this._recommend = custInfo.Rows[0].ToInt32("recommend");
                this._memberId = custInfo.Rows[0].ToInt32("custid");
            }

            return true;
        }

        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        private ActionResult GetWinLossDetailSearch()
        {
            if (!this.CheckSearchCustomer())
            {
                return this.JsonBase(
                    new
                    {
                        viewlevel = 4,
                        supername = this._username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = string.Empty,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    });
            }

            if (this._roleId > 3)
            {
                this._listWinLossDetail = this._reportService.GetWinLossDetail(
                    this._groupname, this._srecommend, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetail.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetail>>(this._cacheList, this._listWinLossDetail, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 4,
                        supername = this._username,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        rows = this._listWinLossDetail,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._roleId == 3)
            {
                this._listWinLossDetail = this._reportService.GetWinLossDetailSuper(
                    this._srecommend, this._mrecommend, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetail.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetail>>(this._cacheList, this._listWinLossDetail, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 3,
                        supername = string.Empty,
                        mastername = this._username,
                        agentname = string.Empty,
                        srecommend = this._srecommend,
                        mrecommend = this._mrecommend,
                        rows = this._listWinLossDetail,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._roleId == 2)
            {
                this._listWinLossDetailMAg = this._reportService.GetWinLossDetailMAg(
                    this._mrecommend, this._recommend, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetailMAg.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetailMAg>>(this._cacheList, this._listWinLossDetailMAg, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 2,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = this._username,
                        srecommend = this._srecommend,
                        mrecommend = this._mrecommend,
                        recommend = this._recommend,
                        rows = this._listWinLossDetailMAg,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else
            {
                this._listWinLossDetailAg = this._reportService.GetWinLossDetailAg(
                    this._recommend, this._memberId, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetailAg.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetailAg>>(this._cacheList, this._listWinLossDetailAg, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 1,
                        supername = string.Empty,
                        mastername = string.Empty,
                        agentname = string.Empty,
                        srecommend = this._srecommend,
                        mrecommend = this._mrecommend,
                        recommend = this._recommend,
                        rows = this._listWinLossDetailAg,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        private ActionResult GetWinLossDetailDownline()
        {
            if (this._viewlevel == 4)
            {
                this._listWinLossDetail = this._reportService.GetWinLossDetailSuper(this._custId, 0, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetail.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetail>>(this._cacheList, this._listWinLossDetail, 120);
                return this.JsonBase(
                    new { viewlevel = 3, srecommend = this._custId, rows = this._listWinLossDetail, sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._viewlevel == 3)
            {
                this._listWinLossDetailMAg = this._reportService.GetWinLossDetailMAg(this._custId, 0, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetailMAg.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetailMAg>>(this._cacheList, this._listWinLossDetailMAg, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 2,
                        srecommend = this._srecommend,
                        mrecommend = this._custId,
                        rows = this._listWinLossDetailMAg,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else
            {
                this._listWinLossDetailAg = this._reportService.GetWinLossDetailAg(this._custId, 0, this._fromDate, this._toDate, this._productIds);
                this._totalPages = (this._listWinLossDetailAg.Count / this._pageSize) + 1;
                this._cache.Set<List<WinLossDetailAg>>(this._cacheList, this._listWinLossDetailAg, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 1,
                        srecommend = this._srecommend,
                        mrecommend = this._mrecommend,
                        recommend = this._custId,
                        rows = this._listWinLossDetailAg,
                        sortingorderlist = this._sortingOrderList,
                        tooltips = new
                        {
                            showAllbet = WinLossDetailHelper.IsShowProduct("showAB", CurrentSession)
                        }
                    },
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }
    }
}