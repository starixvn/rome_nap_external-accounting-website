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
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO.Statement;
    using Fanex.ExternalAccount.Excel.Statement.StatementPage;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.Statement;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.Interface.Statement;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Statement controller.
    /// </summary>
    public class StatementController : BaseController
    {
        #region Init Statement
        private ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private IStatementService _statementService = new StatementService();
        private List<Statement> _listStatement = null;
        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;
        private string _groupname = string.Empty;
        private string _cacheList = string.Empty;
        private int _custId = 0;
        #endregion Init Statement

        [ExternalAccountAuthenticate(Function = eFunctions.Statement, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index()
        {
            ViewBag.IsInternal = Utils.ToBoolean(Request.QueryString["isinternal"]);
            return this.PartialViewBase();
        }

        public void GetRequestPage()
        {
            this._username = CheckRequest.GetStringRequest("username", string.Empty);
            this._custId = CheckRequest.GetIntRequest("custid", 0);
            this._groupname = (CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin) ?
                CheckRequest.GetStringRequest("groupname", string.Empty) : CurrentSession.CurrencyGroup;
            this._fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            this._toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
            this._cacheList = "CacheList" + this._username + this._groupname + this._custId.ToString() + this._fromDate.ToString() + this._toDate.ToString();
        }

        /// <summary>
        /// Gets statement page.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.Statement, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GetStatement()
        {
            this.GetRequestPage();
            if (this._custId > 0 && this.IsBelong(this._groupname, this._custId, null, true))
            {
                return this.LoadDataStatement();
            }
            else
            {
                return this.JsonBase(new { rows = string.Empty, username = this._username }, new Newtonsoft.Json.JsonSerializerSettings());
            }
        }

        /// <summary>
        /// Exports excel.
        /// </summary>
        public void GetExportToExcel()
        {
            this.GetRequestPage();
            if (this._custId > 0 && this.IsBelong(this._groupname, this._custId, null, true))
            {
                // Expired Cache
                if (!Config.IsCache || this._cache.Get<List<Statement>>(this._cacheList) == null)
                {
                    this.GetStatement();
                }

                var builder = new StatementPageBuilder(
                    this._cache.Get<List<Statement>>(this._cacheList),
                    false,
                    this._username,
                    this._fromDate.ToString("MMM, dd yyyy"),
                    this._toDate.ToString("MMM, dd yyyy"));

                var excelStream = builder.Build();
                if (excelStream != null)
                {
                    this.ExportToExcel(excelStream, "Statement_" + this._username + "_" + this._fromDate.ToString("yyyyMMdd") + "_" + this._toDate.ToString("yyyyMMdd") + ".xls");
                    excelStream.Dispose();
                }
            }
        }

        private ActionResult LoadDataStatement()
        {
            this._listStatement = this._statementService.GetStatement(this._custId, this._fromDate, this._toDate);
            this._cache.Set<List<Statement>>(this._cacheList, this._listStatement, 120);
            return this.JsonBase(new { rows = this._listStatement, username = this._username }, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}