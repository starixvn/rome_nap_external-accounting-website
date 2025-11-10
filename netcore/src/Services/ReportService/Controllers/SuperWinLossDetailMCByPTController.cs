using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (SuperWinLossDetailByPTBaseCurrency Read)
    public class SuperWinLossDetailMCByPTController : BaseReportController
    {
        private readonly ISuperWinLossDetailByPTService services;
        private readonly IProductDetailService productDetailService;

        public SuperWinLossDetailMCByPTController(
            ICurrentSession currentSession,
            ISuperWinLossDetailByPTService services,
            IProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.services = services;
            this.productDetailService = productDetailService;
        }

        [HttpGet]
        [Route("Index")]
        public IActionResult Index()
        {
            var noteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);
            return Ok(new { noteSabaOthers });
        }

        [HttpPost]
        [Route("GetData")]
        public IActionResult GetData([FromBody] SuperWinLossDetailByPTMCRequest request)
        {
            var param = new SuperWinLossDetailByPTParam(
                request.GroupId,
                request.GroupName,
                request.Fromdate,
                request.Todate,
                request.ProductIds,
                request.ResourceIds,
                reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId),
                0,
                0,
                request.UserId)
            {
                BaseCurrency = request.BaseCurrency,
                BaseCurrencyName = request.BaseCurrencyName,
                Currencies = request.Currencies
            };

            var viewModel = services.GetSuperWinLossMCDetailByPT(param);
            return Ok(viewModel);
        }

        [HttpPost]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel([FromBody] ExportSuperWinLossDetailByPTMCRequest request)
        {
            // TODO: Implement Excel export functionality
            // var winlossDetail = services.GetSuperWinLossMCDetailByPT(new SuperWinLossDetailByPTParam(...));
            // ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByMCPTReportBuilder(...);
            // var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            // var memoryStream = builder.Build(formatedProducts);
            // if (memoryStream != null)
            // {
            //     return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            // }

            return Ok(new { message = "Excel export not yet implemented", fileName = request.FileName });
        }
    }

    // TODO: Extend ISuperWinLossDetailByPTService with MC methods
    public partial interface ISuperWinLossDetailByPTService
    {
        object GetSuperWinLossMCDetailByPT(SuperWinLossDetailByPTParam param);
    }

    // TODO: Extend SuperWinLossDetailByPTService with MC implementations
    public partial class SuperWinLossDetailByPTService
    {
        public object GetSuperWinLossMCDetailByPT(SuperWinLossDetailByPTParam param)
        {
            // TODO: Implement actual service logic for multi-currency
            return new { winLossByUsernames = new[] { new { username = "user1", amount = 100.0m } }, totalByCurrencies = new[] { new { currency = "USD", total = 100.0m } } };
        }
    }

    // TODO: Extend SuperWinLossDetailByPTRequest with MC properties
    public partial class SuperWinLossDetailByPTRequest
    {
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; } = string.Empty;
        public string Currencies { get; set; } = string.Empty;
    }

    public class SuperWinLossDetailByPTMCRequest : SuperWinLossDetailByPTRequest
    {
    }

    public class ExportSuperWinLossDetailByPTMCRequest : SuperWinLossDetailByPTRequest
    {
        public string FileName { get; set; } = string.Empty;
    }

    // TODO: Extend SuperWinLossDetailByPTParam with MC properties
    public partial class SuperWinLossDetailByPTParam
    {
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; } = string.Empty;
        public string Currencies { get; set; } = string.Empty;
    }
}