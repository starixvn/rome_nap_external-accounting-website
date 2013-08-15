// --------------------------------------------------------------------------
// <copyright file="DailyWinlossController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;

    /// <summary>
    /// Report controller
    /// </summary>
    public class DailyWinlossController : BaseController
    {
        #region Init DailyWinLoss

        private ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private IReportService reportService = new ReportService();
        private List<DailyWinLoss> listDailyWinLoss = null;
        private DateTime winlostDate = DateTime.Today;
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

        #endregion Init DailyWinLoss

        [Authorization(Function = eFunctions.DailyWinloss, Policy = ePolicy.Read)]
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
            this.winlostDate = CheckRequest.GetDateTimeRequest("date", DateTime.Today);
        }

        [Authorization(Function = eFunctions.DailyWinloss, Policy = ePolicy.Read)]
        public ActionResult GetDailyWinLoss()
        {
            this.GetRequestPage();
            this.srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            this.mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            this.recommend = CheckRequest.GetIntRequest("recommend", 0);
            this.cacheList = "CacheList" + this.username + this.groupname + this.custId.ToString() + this.winlostDate.ToString();

            if (this.CheckBelonging())
            {
                if (this.custId != 0)
                {
                    return this.GetDailyWinLossDownline();
                }
                else
                {
                    return this.GetDailyWinLossSearch();
                }
            }
            else
            {
                return this.JsonBase(new { viewlevel = 5, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = string.Empty });
            }
        }

        public void GetExportToExcel()
        {
            this.GetRequestPage();
            this.cacheList = "CacheList" + this.username + this.groupname + this.custId.ToString() + this.winlostDate.ToString();

            if (this.CheckBelonging())
            {
                // Expired Cache
                if (this.cache.Get<List<DailyWinLoss>>(this.cacheList) == null)
                {
                    this.GetDailyWinLoss();
                }

                var builder = new Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport.DailyWinLossReportBuilder(this.cache.Get<List<DailyWinLoss>>(this.cacheList), false, this.viewlevel, CheckRequest.GetStringRequest("date", string.Empty));

                var excelStream = builder.Build();
                if (excelStream != null)
                {
                    this.ExportToExcel(excelStream, LevelName(this.viewlevel) + "DailyWinLoss_" + this.winlostDate.ToString("yyyyMMdd") + ".xls");
                    excelStream.Dispose();
                }
            }
        }             

        private ActionResult GetDailyWinLossSearch()
        {
            if (this.roleId > 3)
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLoss(this.winlostDate, this.srecommend, this.groupname);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 4, supername = this.username, mastername = string.Empty, agentname = string.Empty, rows = this.listDailyWinLoss });
            }
            else if (this.roleId == 3)
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossSuper(this.winlostDate, this.srecommend, this.mrecommend);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 3, supername = string.Empty, mastername = this.username, agentname = string.Empty, srecommend = this.srecommend, mrecommend = this.mrecommend, rows = this.listDailyWinLoss });
            }
            else if (this.roleId == 2)
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossMaster(this.winlostDate, this.mrecommend, this.recommend);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 2, supername = string.Empty, mastername = string.Empty, agentname = this.username, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = this.listDailyWinLoss });
            }
            else
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossAgent(this.winlostDate, this.recommend, this.memberId);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 1, supername = string.Empty, mastername = string.Empty, agentname = string.Empty, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.recommend, rows = this.listDailyWinLoss });
            }
        }

        private ActionResult GetDailyWinLossDownline()
        {
            if (this.viewlevel == 4)
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossSuper(this.winlostDate, this.custId, 0);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 3, srecommend = this.custId, rows = this.listDailyWinLoss });
            }
            else if (this.viewlevel == 3)
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossMaster(this.winlostDate, this.custId, 0);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 2, srecommend = this.srecommend, mrecommend = this.custId, rows = this.listDailyWinLoss });
            }
            else
            {
                this.listDailyWinLoss = this.reportService.GetDailyWinLossAgent(this.winlostDate, this.custId, 0);
                this.cache.Set<List<DailyWinLoss>>(this.cacheList, this.listDailyWinLoss, 120);
                return this.JsonBase(new { viewlevel = 1, srecommend = this.srecommend, mrecommend = this.mrecommend, recommend = this.custId, rows = this.listDailyWinLoss });
            }
        }
    }
}