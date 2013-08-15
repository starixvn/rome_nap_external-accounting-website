// --------------------------------------------------------------------------
// <copyright file="WinLossDetailController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Report controller
    /// </summary>
    public class WinLossDetailController : BaseController
    {
        #region Init WinLossDetail

        private ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private IReportService reportService = new ReportService();
        private List<WinLossDetail> listWinLossDetail = null;
        private List<WinLossDetailMAg> listWinLossDetailMAg = null;
        private List<WinLossDetailAg> listWinLossDetailAg = null;
        private ArrayList sortingOrderList = null;
        private DateTime fromDate = DateTime.Today;
        private DateTime toDate = DateTime.Today;
        private string username = string.Empty;
        private string groupname = string.Empty;
        private string cacheList = string.Empty;
        private int roleId = 5;
        private int viewlevel = 5;
        private int srecommend = 0;
        private int mrecommend = 0;
        private int recommend = 0;
        private int memberId = 0;
        private int custId = 0;
        private string searchName = string.Empty;
        private ICollection<KeyValuePair<string, bool>> filterParams = null;
        int pageSize = 50;
        int totalPages = 1;

        #endregion Init WinLossDetail

        [Authorization(Function = eFunctions.WinlossDetail, Policy = ePolicy.Read)]
        public ActionResult Index()
        {
            return this.PartialViewBase();
        }

        private bool CheckBelonging()
        {
            if (!string.IsNullOrEmpty(this.username))
            {
                DataTable customerInfo = this.reportService.GetCustomerInfo(this.groupname, this.custId, this.username, true);
                if (customerInfo != null)
                {
                    this.roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                    if (this.roleId == 4)
                    {
                        this.srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                    }
                    else if (this.roleId == 3)
                    {
                        this.srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this.mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                    }
                    else if (this.roleId == 2)
                    {
                        this.srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this.mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                        this.recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                    }
                    else
                    {
                        this.srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this.mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                        this.recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                        this.memberId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }

        public void GetRequestPage()
        {
            this.username = CheckRequest.GetStringRequest("username", string.Empty);
            this.viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            this.custId = CheckRequest.GetIntRequest("custid", 0);
            this.groupname = (CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin) ? CheckRequest.GetStringRequest("groupname", string.Empty) : CurrentSession.CurrencyGroup;
            this.fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            this.toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
        }

        [Authorization(Function = eFunctions.WinlossDetail, Policy = ePolicy.Read)]
        public ActionResult GetData()
        {
            this.GetRequestPage();
            this.srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            this.mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            this.recommend = CheckRequest.GetIntRequest("recommend", 0);
            this.filterParams = new FilterController().AsCollection();
            this.cacheList = "CacheList" + this.username + this.groupname + this.custId.ToString() + this.fromDate.ToString() + this.toDate.ToString();

            if (this.CheckBelonging())
            {
                this.sortingOrderList = this.reportService.GetSortingOrderList(this.groupname);

                if (this.custId != 0)
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
                return this.JsonBase(new { viewlevel = 5, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = string.Empty, sortingorderlist = this.sortingOrderList });
            }
        }

        public void GetExportToExcelWinLossDetail()
        {
            this.GetRequestPage();
            this.cacheList = "CacheList" + this.username + this.groupname + this.custId.ToString() + this.fromDate.ToString() + this.toDate.ToString();

            if (this.CheckBelonging())
            {
                if (this.viewlevel == 1)
                {
                    // Expired Cache
                    if (cache.Get<List<WinLossDetailAg>>(this.cacheList) == null)
                    {
                        this.GetData();
                    }

                    var builder = new Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport.WinLossDetailReportAgBuilder(cache.Get<List<WinLossDetailAg>>(this.cacheList), false, this.viewlevel, this.fromDate.ToString("MMM, dd yyyy"), this.toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build();
                    if (excelStream != null)
                    {
                        this.ExportToExcel(excelStream, "MemberWinLossDetail_" + this.fromDate.ToString("yyyyMMdd") + "_" + this.toDate.ToString("yyyyMMdd") + ".xls");
                        excelStream.Dispose();
                    }
                }
                else if (this.viewlevel == 2)
                {
                    // Expired Cache
                    if (cache.Get<List<WinLossDetailMAg>>(this.cacheList) == null)
                    {
                        this.GetData();
                    }

                    var builder = new Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport.WinLossDetailReportMAgBuilder(cache.Get<List<WinLossDetailMAg>>(this.cacheList), false, this.viewlevel, this.fromDate.ToString("MMM, dd yyyy"), this.toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build();
                    if (excelStream != null)
                    {
                        this.ExportToExcel(excelStream, LevelName(this.viewlevel) + "WinLossDetail_" + this.fromDate.ToString("yyyyMMdd") + "_" + this.toDate.ToString("yyyyMMdd") + ".xls");
                        excelStream.Dispose();
                    }
                }
                else
                {
                    // Expired Cache
                    if (cache.Get<List<WinLossDetail>>(this.cacheList) == null)
                    {
                        this.GetData();
                    }

                    var builder = new Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport.WinLossDetailReportBuilder(cache.Get<List<WinLossDetail>>(this.cacheList), false, this.viewlevel, this.fromDate.ToString("MMM, dd yyyy"), this.toDate.ToString("MMM, dd yyyy"));
                    var excelStream = builder.Build();
                    if (excelStream != null)
                    {
                        this.ExportToExcel(excelStream, LevelName(this.viewlevel) + "WinLossDetail_" + this.fromDate.ToString("yyyyMMdd") + "_" + this.toDate.ToString("yyyyMMdd") + ".xls");
                        excelStream.Dispose();
                    }
                }
            }
        }

        private ActionResult GetWinLossDetailSearch()
        {
            if (this.roleId > 3)
            {
                this.listWinLossDetail = this.reportService.GetWinLossDetail(this.groupname, this.srecommend, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetail.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetail>>(this.cacheList, this.listWinLossDetail, 120);
                return this.JsonBase(new { viewlevel = 4, supername = this.username, mastername = string.Empty, agentname = string.Empty, rows = this.listWinLossDetail, sortingorderlist = this.sortingOrderList });
            }
            else if (this.roleId == 3)
            {
                this.listWinLossDetail = this.reportService.GetWinLossDetailSuper(this.srecommend, this.mrecommend, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetail.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetail>>(this.cacheList, this.listWinLossDetail, 120);
                return this.JsonBase(new { viewlevel = 3, supername = string.Empty, mastername = this.username, agentname = string.Empty, srecommend = this.srecommend, mrecommend = this.mrecommend, rows = this.listWinLossDetail, sortingorderlist = this.sortingOrderList });
            }
            else if (this.roleId == 2)
            {
                this.listWinLossDetailMAg = this.reportService.GetWinLossDetailMAg(this.mrecommend, this.recommend, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetailMAg.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetailMAg>>(this.cacheList, this.listWinLossDetailMAg, 120);
                return this.JsonBase(new { viewlevel = 2, supername = string.Empty, mastername = string.Empty, agentname = this.username, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = this.listWinLossDetailMAg, sortingorderlist = this.sortingOrderList });
            }
            else
            {
                this.listWinLossDetailAg = this.reportService.GetWinLossDetailAg(this.recommend, this.memberId, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetailAg.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetailAg>>(this.cacheList, this.listWinLossDetailAg, 120);
                return this.JsonBase(new { viewlevel = 1, supername = string.Empty, mastername = string.Empty, agentname = string.Empty, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = this.listWinLossDetailAg, sortingorderlist = this.sortingOrderList });
            }
        }

        private ActionResult GetWinLossDetailDownline()
        {
            IReportService reportService = new ReportService();
            if (this.viewlevel == 4)
            {
                this.listWinLossDetail = this.reportService.GetWinLossDetailSuper(this.custId, 0, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetail.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetail>>(this.cacheList, this.listWinLossDetail, 120);
                return this.JsonBase(new { viewlevel = 3, srecommend = this.custId, rows = this.listWinLossDetail, sortingorderlist = this.sortingOrderList });
            }
            else if (this.viewlevel == 3)
            {
                this.listWinLossDetailMAg = this.reportService.GetWinLossDetailMAg(this.custId, 0, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetailMAg.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetailMAg>>(this.cacheList, this.listWinLossDetailMAg, 120);
                return this.JsonBase(new { viewlevel = 2, srecommend = this.srecommend, mrecommend = this.custId, rows = this.listWinLossDetailMAg, sortingorderlist = this.sortingOrderList });
            }
            else
            {
                this.listWinLossDetailAg = this.reportService.GetWinLossDetailAg(this.custId, 0, this.fromDate, this.toDate, this.filterParams);
                this.totalPages = this.listWinLossDetailAg.Count / this.pageSize + 1;
                cache.Set<List<WinLossDetailAg>>(this.cacheList, this.listWinLossDetailAg, 120);
                return this.JsonBase(new { viewlevel = 1, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.custId, rows = this.listWinLossDetailAg, sortingorderlist = this.sortingOrderList });
            }
        }
    }
}