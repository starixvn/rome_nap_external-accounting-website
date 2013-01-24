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
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;

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
        private string groupname = string.Empty;
        private string cacheList = string.Empty;
        private int custId = 0;

        #endregion Init Statement

        [Authorization(Function = eFunctions.Statement, Policy = ePolicy.Read)]
        public ActionResult Index()
        {
            //return GetStatement();
            return this.ViewBase();
        }

        public void GetRequestPage()
        {
            this.custId = CheckRequest.GetIntRequest("custid", 0);
            this.groupname = CheckRequest.GetStringRequest("groupname", "ACCDDD");
            this.fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            this.toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
            this.cacheList = "CacheList" + this.groupname + this.custId.ToString() + this.fromDate.ToString() + this.toDate.ToString();
        }

        [Authorization(Function = eFunctions.Statement, Policy = ePolicy.Read)]
        public ActionResult GetStatement()
        {
            this.GetRequestPage();
            if (custId > 0)
            {
                return this.LoadDateStatement();
            }
            else
            {
                return this.JsonBase(new { rows = string.Empty });
            }
        }

        public void GetExportToExcel()
        {
            this.GetRequestPage();

            // Expired Cache
            if (this.cache.Get<List<Statement>>(this.cacheList) == null)
            {
                this.GetStatement();
            }

            var builder = new Fanex.ExternalAccount.Excel.Statement.StatementPage.StatementPageBuilder(this.cache.Get<List<Statement>>(this.cacheList), false, this.fromDate.ToString("MMM, dd yyyy"), this.toDate.ToString("MMM, dd yyyy"));

            var excelStream = builder.Build();
            if (excelStream != null)
            {
                this.ExportToExcel(excelStream, "Statement_" + this.fromDate.ToString("yyyyMMdd") + "_" + this.toDate.ToString("yyyyMMdd") + ".xls");
                excelStream.Dispose();
            }
        }

        private ActionResult LoadDateStatement()
        {
            this.listStatement = this.StatementService.GetStatement(this.custId, this.fromDate, this.toDate);
            cache.Set<List<Statement>>(this.cacheList, this.listStatement, 120);
            return this.JsonBase(new { rows = this.listStatement });
        }
    }
}