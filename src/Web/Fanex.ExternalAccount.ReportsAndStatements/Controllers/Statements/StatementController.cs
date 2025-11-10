using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Statements
{
    public class StatementController : Controller
    {
        private readonly IMemoryCache _cache;
        private readonly IStatementService _statementService;
        private string _cacheName = string.Empty;
        private int _custId;
        private DateTime _fromDate = DateTime.Today;
        private DateTime _toDate = DateTime.Today;
        private string _username = string.Empty;

        public StatementController(IMemoryCache cache, IStatementService statementService)
        {
            _cache = cache;
            _statementService = statementService ?? new StatementService();
        }

        public IActionResult Index()
        {
            ViewBag.IsInternal = HttpContext.Request.Query["isinternal"].ToString().ToLower() == "true";
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetStatement()
        {
            GetRequestPage();

            if (_custId > 0)
            {
                return LoadDataStatement();
            }

            return Json(new { rows = string.Empty, username = _username });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetExportToExcel()
        {
            GetRequestPage();
            if (_custId > 0)
            {
                if (!_cache.TryGetValue(_cacheName, out List<Statement> cachedValue))
                {
                    LoadDataStatement();
                }
                _cache.TryGetValue(_cacheName, out cachedValue);

                // Export logic would go here
                // var builder = new StatementPageBuilder(...);
                // var excelStream = builder.Build();
                // var fileName = $"Statement_{_username}_{_fromDate.ToString("yyyyMMdd")}_{_toDate.ToString("yyyyMMdd")}.xls";
                // return File(excelStream, "application/vnd.ms-excel", fileName);
            }

            return BadRequest();
        }

        private IActionResult LoadDataStatement()
        {
            var listStatement = _statementService.GetStatement(_custId, _fromDate, _toDate);
            _cache.Set(_cacheName, listStatement, TimeSpan.FromMinutes(30));

            return Json(new { rows = listStatement, username = _username });
        }

        private void GetRequestPage()
        {
            _username = HttpContext.Request.Form["username"].ToString() ?? string.Empty;
            _custId = int.TryParse(HttpContext.Request.Form["custid"], out var custId) ? custId : 0;
            _fromDate = DateTime.TryParse(HttpContext.Request.Form["fromdate"], out var fromDate) ? fromDate : DateTime.Today;
            _toDate = DateTime.TryParse(HttpContext.Request.Form["todate"], out var toDate) ? toDate : DateTime.Today;
            _cacheName = $"CacheList{_username}{GetGroupName()}{_custId}{_fromDate}{_toDate}";
        }

        private string GetGroupName() => string.Empty; // Placeholder
    }

    // Placeholder classes
    public interface IStatementService
    {
        List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate);
    }

    public class StatementService : IStatementService
    {
        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            return new List<Statement>();
        }
    }

    public class Statement
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public decimal Amount { get; set; }
    }
}
