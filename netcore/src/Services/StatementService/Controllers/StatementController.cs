using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Statement
{
    [ApiController]
    [Route("api/[controller]")]
    public class StatementController : ControllerBase
    {
        private readonly IMemoryCache _cache;
        private readonly IStatementService _statementService;

        public StatementController(IMemoryCache cache, IStatementService statementService)
        {
            _cache = cache;
            _statementService = statementService;
        }

        [HttpGet]
        [Route("Index")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Index([FromQuery] bool isInternal = false)
        {
            // TODO: Implement proper view rendering or API response
            return Ok(new { isInternal });
        }

        [HttpGet]
        [Route("GetStatement")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult GetStatement([FromQuery] string username, [FromQuery] int custId,
                                        [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var from = fromDate ?? DateTime.Today;
            var to = toDate ?? DateTime.Today;

            if (custId > 0)
            {
                return LoadDataStatement(custId, from, to, username);
            }

            return Ok(new { rows = string.Empty, username });
        }

        [HttpGet]
        [Route("ExportToExcel")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult ExportToExcel([FromQuery] string username, [FromQuery] int custId,
                                         [FromQuery] DateTime? fromDate, [FromQuery] DateTime? toDate)
        {
            var from = fromDate ?? DateTime.Today;
            var to = toDate ?? DateTime.Today;

            if (custId <= 0)
            {
                return BadRequest("Invalid customer ID");
            }

            var cacheName = $"CacheList{username}{custId}{from}{to}";
            if (!_cache.TryGetValue<List<Statement>>(cacheName, out var cachedValue))
            {
                var listStatement = _statementService.GetStatement(custId, from, to);
                _cache.Set(cacheName, listStatement, TimeSpan.FromMinutes(30));
                cachedValue = listStatement;
            }

            // TODO: Implement Excel export functionality
            // var builder = new StatementPageBuilder(
            //     cachedValue,
            //     false,
            //     username,
            //     from.ToString("MMM, dd yyyy"),
            //     to.ToString("MMM, dd yyyy"));

            // var excelStream = builder.Build();
            // var fileName = $"Statement_{username}_{from.ToString("yyyyMMdd")}_{to.ToString("yyyyMMdd")}.xls";
            // return File(excelStream, "application/vnd.ms-excel", fileName);

            return Ok(new { message = "Excel export not yet implemented", count = cachedValue?.Count ?? 0 });
        }

        private IActionResult LoadDataStatement(int custId, DateTime fromDate, DateTime toDate, string username)
        {
            var listStatement = _statementService.GetStatement(custId, fromDate, toDate);
            var cacheName = $"CacheList{username}{custId}{fromDate}{toDate}";
            _cache.Set(cacheName, listStatement, TimeSpan.FromMinutes(30));

            return Ok(new { rows = listStatement, username });
        }
    }

    // TODO: Define proper Statement DTO
    public class Statement
    {
        // Placeholder - needs to be defined based on actual Statement model
    }

    // TODO: Define proper IStatementService interface
    public interface IStatementService
    {
        List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate);
    }

    // TODO: Implement StatementService
    public class StatementService : IStatementService
    {
        public List<Statement> GetStatement(int custId, DateTime fromDate, DateTime toDate)
        {
            // TODO: Implement actual service logic
            return new List<Statement>();
        }
    }
}