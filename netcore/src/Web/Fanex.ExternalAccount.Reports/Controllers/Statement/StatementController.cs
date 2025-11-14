// namespace Fanex.ExternalAccount.Controllers.Statement
// {
//     using Caching;
//     using Core.Base;
//     using Core.Context;
//     using Core.Shared;
//     using CustomerSetting.Security.Authenticate;
//     using DTO.Statement;
//     using Excel.Statement.StatementPage;
//     using Fanex.Caching.Memory;
//     using Newtonsoft.Json;
//     using Service.Impl.Statement;
//     using Service.Interface.Statement;
//     using System;
//     using System.Collections.Generic;
//     using System.Web.Mvc;
//     using Utilities;
//
//     public class StatementController : BaseController
//     {
//         private readonly ICacheService _cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));
//         private readonly IStatementService _statementService = new StatementService();
//         private string _cacheName = string.Empty;
//         private int _custId;
//         private DateTime _fromDate = DateTime.Today;
//         private DateTime _toDate = DateTime.Today;
//         private string _username = string.Empty;
//
//         [ExternalAccountAuthenticate(Function = eFunctions.Statement,
//             Policy = AuthenticateAction.AuthenticationType.Read)]
//         public IActionResult Index()
//         {
//             ViewBag.IsInternal = Utils.ToBoolean(Request.QueryString["isinternal"]);
//             return PartialViewBase();
//         }
//
//         [ExternalAccountAuthenticate(Function = eFunctions.Statement,
//             Policy = AuthenticateAction.AuthenticationType.Read)]
//         [ValidateAntiForgeryToken]
//         public IActionResult GetStatement()
//         {
//             GetRequestPage();
//
//             if (_custId > 0)
//             {
//                 return LoadDataStatement();
//             }
//
//             return JsonBase(new { rows = string.Empty, username = _username }, new JsonSerializerSettings());
//         }
//
//         [ExternalAccountAuthenticate(Function = eFunctions.Statement,
//             Policy = AuthenticateAction.AuthenticationType.Read)]
//         [ValidateAntiForgeryToken]
//         public void GetExportToExcel()
//         {
//             GetRequestPage();
//             if (_custId > 0)
//             {
//                 if (!Config.IsCache || _cache.TryGet<List<Statement>>(_cacheName, out var value))
//                 {
//                     LoadDataStatement();
//                 }
//                 _cache.TryGet<List<Statement>>(_cacheName, out var cachedValue);
//
//                 var builder = new StatementPageBuilder(
//                     cachedValue,
//                     false,
//                     _username,
//                     _fromDate.ToString("MMM, dd yyyy"),
//                     _toDate.ToString("MMM, dd yyyy"));
//
//                 var excelStream = builder.Build();
//                 var fileName = $"Statement_{_username}_{_fromDate.ToString("yyyyMMdd")}_{_toDate.ToString("yyyyMMdd")}.xls";
//                 ExportToExcel(excelStream, fileName);
//             }
//         }
//
//         private ActionResult LoadDataStatement()
//         {
//             var listStatement = _statementService.GetStatement(_custId, _fromDate, _toDate);
//             _cache.Set(_cacheName, listStatement, new CacheItemOptions());
//
//             return JsonBase(new { rows = listStatement, username = _username }, new JsonSerializerSettings());
//         }
//
//         private void GetRequestPage()
//         {
//             _username = CheckRequest.GetStringRequest("username", string.Empty);
//             _custId = CheckRequest.GetIntRequest("custid", 0);
//             _fromDate = CheckRequest.GetDateTimeRequest("fromdate", DateTime.Today);
//             _toDate = CheckRequest.GetDateTimeRequest("todate", DateTime.Today);            
//             _cacheName = $"CacheList{_username}{ExtRequest.GetGroupName()}{_custId}{_fromDate}{_toDate}";
//         }
//     }
// }