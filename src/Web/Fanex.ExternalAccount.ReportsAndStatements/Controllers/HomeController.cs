using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.ReportsAndStatements.Models;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Configuration;
using Microsoft.Extensions.Configuration;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private readonly IConfiguration _configuration;
    private readonly StoredProcedureService _spService;

    public HomeController(ILogger<HomeController> logger, IConfiguration configuration, StoredProcedureService spService)
    {
        _logger = logger;
        _configuration = configuration;
        _spService = spService;
    }

    public IActionResult Index()
    {
        // Test configuration access
        var environment = _configuration["AppSettings:Environment"];
        var systemId = _configuration["AppSettings:SystemId"];
        var connectionString = _configuration.GetConnectionString("DBCUST.CustDB.wsv_xnap");

        _logger.LogInformation("Configuration Test - Environment: {Env}, SystemId: {SysId}, HasConnectionString: {HasConn}",
            environment, systemId, !string.IsNullOrEmpty(connectionString));

        // Test stored procedure service
        var spCount = _spService.GetAllStoredProcedures().Count();
        _logger.LogInformation("Loaded {Count} stored procedures", spCount);

        return View();
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
