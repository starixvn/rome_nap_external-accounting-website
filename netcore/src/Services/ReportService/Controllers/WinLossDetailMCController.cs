using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (WinLossDetailsBaseCurrency Read)
    public class WinLossDetailMCController : BaseReportController
    {
        private readonly IWinLossDetailService winLossDetailService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailMCController(
            ICurrentSession currentSession,
            IWinLossDetailService winLossDetailService,
            IProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.winLossDetailService = winLossDetailService;
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
        public IActionResult GetData([FromBody] WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Ok(viewModel);
        }

        [HttpPost]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel([FromBody] ExportWinLossMCDetailRequest request)
        {
            // TODO: Implement Excel export functionality
            // IWinLossDetailReportBuilder builder;
            // request.Viewlevel++;
            // var winlossDetail = GetDataFromDB(request);
            // ... complex logic with different builders based on viewlevel ...
            // var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            // var memoryStream = builder.Build(formatedProducts);
            // if (memoryStream != null)
            // {
            //     return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            // }

            return Ok(new { message = "Excel export not yet implemented", fileName = request.FileName });
        }

        private object GetDataFromDB(WinLossDetailRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                rngCasino = reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId)
            };

            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, toolTipsProducts))
                : winLossDetailService.DrillDownWinLossMCDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }
    }

    // TODO: Extend IWinLossDetailService with MC methods
    public partial interface IWinLossDetailService
    {
        object DrillDownWinLossMCDetail(DrillDownWinLossDetailRequest request);
    }

    // TODO: Extend WinLossDetailService with MC implementations
    public partial class WinLossDetailService
    {
        public object DrillDownWinLossMCDetail(DrillDownWinLossDetailRequest request)
        {
            // TODO: Implement actual service logic for multi-currency
            return new { rows = "MC drill down results", request = request };
        }
    }

    // TODO: Extend WinLossDetailRequest with base currency properties
    public partial class WinLossDetailRequest
    {
        public int BaseCurrency { get; set; }
        public string BaseCurrencyName { get; set; } = string.Empty;
    }

    public class ExportWinLossMCDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{Viewlevel.DisplayRoleName()}WinLossDetail_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}_in_{(BaseCurrency == 0 ? "customer_currency" : BaseCurrencyName)}.xls";
    }
}