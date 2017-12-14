namespace Fanex.ExternalAccount.Controllers.Statement
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Caching;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using DTO.Statement;
    using Excel.Statement.StatementPage;
    using Newtonsoft.Json;
    using Service.Impl.Statement;
    using Service.Interface.Statement;
    using Utilities;

    public class StatementController : BaseController
    {
        private readonly ICache _cache = CacheService.CreateInstance(CacheType.MemoryCache);
        private readonly IStatementService _statementService = new StatementService();
        private string _cacheList = string.Empty;
        private int _custId;
        private DateTime _fromDate = DateTime.Today;
        private string _groupname = string.Empty;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;

        [ExternalAccountAuthenticate(Function = eFunctions.Statement,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index()
        {
            ViewBag.IsInternal = Utils.ToBoolean(Request.QueryString["isinternal"]);
            return PartialViewBase();
        }

        public void GetRequestPage()
        {
            _username = CheckRequest.GetStringRequest("username", string.Empty);
            _custId = CheckRequest.GetIntRequest("custid", 0);
            _groupname = CurrentSession.IsInternalAdmin || CurrentSession.IsITAdmin
                ? CheckRequest.GetStringRequest("groupname", string.Empty)
                : CurrentSession.CurrencyGroup;
            _fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
            _toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);
            _cacheList = "CacheList" + _username + _groupname + _custId + _fromDate + _toDate;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Statement,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public ActionResult GetStatement()
        {
            GetRequestPage();

            if (_custId > 0)
            {
                return LoadDataStatement();
            }

            return JsonBase(new { rows = string.Empty, username = _username }, new JsonSerializerSettings());
        }

        [ExternalAccountAuthenticate(Function = eFunctions.Statement,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        [AntiXSRFFilter]
        public void GetExportToExcel()
        {
            GetRequestPage();
            if (_custId > 0)
            {
                if (!Config.IsCache || _cache.Get<List<Statement>>(_cacheList) == null)
                {
                    GetStatement();
                }

                var builder = new StatementPageBuilder(
                    _cache.Get<List<Statement>>(_cacheList),
                    false,
                    _username,
                    _fromDate.ToString("MMM, dd yyyy"),
                    _toDate.ToString("MMM, dd yyyy"));

                var excelStream = builder.Build();
                if (excelStream != null)
                {
                    ExportToExcel(excelStream,
                        "Statement_" + _username + "_" + _fromDate.ToString("yyyyMMdd") + "_" +
                        _toDate.ToString("yyyyMMdd") + ".xls");
                    excelStream.Dispose();
                }
            }
        }

        private ActionResult LoadDataStatement()
        {
            var listStatement = _statementService.GetStatement(_custId, _fromDate, _toDate);
            _cache.Set(_cacheList, listStatement);
            return JsonBase(new { rows = listStatement, username = _username }, new JsonSerializerSettings());
        }
    }
}