// --------------------------------------------------------------------------
// <copyright file="StatementController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Statement
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.Statement;
    using Fanex.ExternalAccount.Service.Impl.Statement;
    using Fanex.ExternalAccount.Service.Interface.Statement;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.Impl.Report;

    /// <summary>
    /// Statement controller
    /// </summary>
    public class StatementController : BaseController
    {
        #region Init Statement
        private ICache cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private IStatementService StatementService = new StatementService();
        private List<Statement> listStatement = null;
        private DateTime fromDate = DateTime.Today;
        private DateTime toDate = DateTime.Today;
        private string username = string.Empty;
        private string groupname = string.Empty;
        private string cacheList = string.Empty;
        private int custId = 0;
        #endregion Init Statement

        [Authorization(Function = eFunctions.Statement, Policy = ePolicy.Read, IsAjaxRequest = false)]
        public ActionResult Index()
        {
            return this.PartialViewBase();
        }

        private bool CheckBelonging()
        {
            IReportService reportService = new ReportService();
            DataTable customerInfo = reportService.GetCustomerInfo(this.groupname, this.custId, string.Empty, true);
            if (customerInfo != null)
            {
                this.username = Utils.ToString(customerInfo.Rows[0]["username"]);
                return true;
            }
            else
            {
                return false;
            }
        }

        public void GetRequestPage()
        {            
            this.custId = CheckRequest.GetIntRequest("custid", 0);
            this.groupname = (CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin) ? CheckRequest.GetStringRequest("groupname", string.Empty) : CurrentSession.CurrencyGroup;
            this.fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            this.toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
            this.cacheList = "CacheList" + this.groupname + this.custId.ToString() + this.fromDate.ToString() + this.toDate.ToString();
        }

        [Authorization(Function = eFunctions.Statement, Policy = ePolicy.Read)]
        public ActionResult GetStatement()
        {
            this.GetRequestPage();
            if (custId > 0 && CheckBelonging())
            {
                return this.LoadDataStatement();
            }
            else
            {
                return this.JsonBase(new { rows = string.Empty, username = this.username });
            }
        }

        public void GetExportToExcel()
        {
            this.GetRequestPage();
            if (custId > 0 && CheckBelonging())
            {
                // Expired Cache
                if (this.cache.Get<List<Statement>>(this.cacheList) == null)
                {
                    this.GetStatement();
                }

                var builder = new Fanex.ExternalAccount.Excel.Statement.StatementPage.StatementPageBuilder(this.cache.Get<List<Statement>>(this.cacheList), false, this.username, this.fromDate.ToString("MMM, dd yyyy"), this.toDate.ToString("MMM, dd yyyy"));

                var excelStream = builder.Build();
                if (excelStream != null)
                {
                    this.ExportToExcel(excelStream, "Statement_" + this.username + "_" + this.fromDate.ToString("yyyyMMdd") + "_" + this.toDate.ToString("yyyyMMdd") + ".xls");
                    excelStream.Dispose();
                }
            }
        }

        private ActionResult LoadDataStatement()
        {
            this.listStatement = this.StatementService.GetStatement(this.custId, this.fromDate, this.toDate);
            cache.Set<List<Statement>>(this.cacheList, this.listStatement, 120);
            return this.JsonBase(new { rows = this.listStatement, username = this.username });
        }
    }
}