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
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.DailyWinLossReport;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Report controller.
    /// </summary>
    public class DailyWinlossController : BaseController
    {
        #region Init DailyWinLoss

        private ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private IReportService _reportService = new ReportService();
        private List<DailyWinLoss> _listDailyWinLoss = null;
        private DateTime _winlostDate = DateTime.Today;
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
        private string _searchName = string.Empty;

        #endregion Init DailyWinLoss

        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index()
        {
            return this.PartialViewBase();
        }

        /// <summary>
        /// Gets daily win loss page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.DailyWinloss, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GetDailyWinLoss()
        {
            this.GetRequestPage();
            this._srecommend = CheckRequest.GetIntRequest("srecommend", 0);
            this._mrecommend = CheckRequest.GetIntRequest("mrecommend", 0);
            this._recommend = CheckRequest.GetIntRequest("recommend", 0);
            this._cacheList = "CacheList" + this._username + this._groupname + this._custId.ToString() + this._winlostDate.ToString();

            if (this.CheckBelonging())
            {
                if (this._custId != 0)
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
                return this.JsonBase(
                    new { viewlevel = 5, srecommend = this._srecommend, mrecommend = this._mrecommend, recommend = this._recommend, rows = string.Empty }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        /// <summary>
        /// Exports excel.
        /// </summary>
        public void GetExportToExcel()
        {
            this.GetRequestPage();
            this._cacheList = "CacheList" + this._username + this._groupname + this._custId.ToString() + this._winlostDate.ToString();

            if (this.CheckBelonging())
            {
                // Expired Cache
                if (!Config.IsCache || this._cache.Get<List<DailyWinLoss>>(this._cacheList) == null)
                {
                    this.GetDailyWinLoss();
                }

                var builder = new DailyWinLossReportBuilder(
                    this._cache.Get<List<DailyWinLoss>>(this._cacheList), 
                    false, 
                    this._viewlevel, 
                    CheckRequest.GetStringRequest("date", string.Empty));

                var excelStream = builder.Build();
                if (excelStream != null)
                {
                    this.ExportToExcel(excelStream, this.LevelName(this._viewlevel) + "DailyWinLoss_" + this._winlostDate.ToString("yyyyMMdd") + ".xls");
                    excelStream.Dispose();
                }
            }
        }

        public void GetRequestPage()
        {
            this._username = CheckRequest.GetStringRequest("username", string.Empty);
            this._viewlevel = CheckRequest.GetIntRequest("viewlevel", 0);
            this._custId = CheckRequest.GetIntRequest("custid", 0);
            this._groupname = (CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin) ? CheckRequest.GetStringRequest("groupname", string.Empty) : 
                CurrentSession.CurrencyGroup;
            this._winlostDate = CheckRequest.GetDateTimeRequest("date", DateTime.Today);
        }

        /// <summary>
        /// Search daily win loss.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        private ActionResult GetDailyWinLossSearch()
        {
            if (this._roleId > 3)
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLoss(this._winlostDate, this._srecommend, this._groupname);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new { viewlevel = 4, supername = this._username, mastername = string.Empty, agentname = string.Empty, rows = this._listDailyWinLoss }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._roleId == 3)
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossSuper(this._winlostDate, this._srecommend, this._mrecommend);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 3, supername = string.Empty, mastername = this._username, agentname = string.Empty, 
                        srecommend = this._srecommend, mrecommend = this._mrecommend, rows = this._listDailyWinLoss
                    }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._roleId == 2)
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossMaster(this._winlostDate, this._mrecommend, this._recommend);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 2, supername = string.Empty, mastername = string.Empty, agentname = this._username,
                        srecommend = this._srecommend, mrecommend = this._mrecommend, recommend = this._recommend, rows = this._listDailyWinLoss
                    }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossAgent(this._winlostDate, this._recommend, this._memberId);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 1, supername = string.Empty, mastername = string.Empty, agentname = string.Empty, srecommend = this._srecommend,
                        mrecommend = this._mrecommend, recommend = this._recommend, rows = this._listDailyWinLoss
                    }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        /// <summary>
        /// Gets daily win loss of down-line.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        private ActionResult GetDailyWinLossDownline()
        {
            if (this._viewlevel == 4)
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossSuper(this._winlostDate, this._custId, 0);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new { viewlevel = 3, srecommend = this._custId, rows = this._listDailyWinLoss }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else if (this._viewlevel == 3)
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossMaster(this._winlostDate, this._custId, 0);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new
                    {
                        viewlevel = 2,
                        srecommend = this._srecommend,
                        mrecommend = this._custId,
                        rows = this._listDailyWinLoss
                    }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
            else
            {
                this._listDailyWinLoss = this._reportService.GetDailyWinLossAgent(this._winlostDate, this._custId, 0);
                this._cache.Set<List<DailyWinLoss>>(this._cacheList, this._listDailyWinLoss, 120);
                return this.JsonBase(
                    new { viewlevel = 1, srecommend = _srecommend, mrecommend = _mrecommend, recommend = _custId, rows = _listDailyWinLoss }, 
                    new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        /// <summary>
        /// Checks belonging.
        /// </summary>
        /// <returns>Boolean object.</returns>
        private bool CheckBelonging()
        {
            if (!string.IsNullOrEmpty(this._username))
            {
                DataTable customerInfo = this._reportService.GetCustomerInfo(this._groupname, this._custId, this._username, true);
                if (customerInfo != null)
                {
                    this._roleId = Utils.ToInt32(customerInfo.Rows[0]["roleid"]);
                    if (this._roleId == 4)
                    {
                        this._srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                    }
                    else if (this._roleId == 3)
                    {
                        this._srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this._mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                    }
                    else if (this._roleId == 2)
                    {
                        this._srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this._mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                        this._recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                    }
                    else
                    {
                        this._srecommend = Utils.ToInt32(customerInfo.Rows[0]["srecommend"]);
                        this._mrecommend = Utils.ToInt32(customerInfo.Rows[0]["mrecommend"]);
                        this._recommend = Utils.ToInt32(customerInfo.Rows[0]["recommend"]);
                        this._memberId = Utils.ToInt32(customerInfo.Rows[0]["custid"]);
                    }
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}