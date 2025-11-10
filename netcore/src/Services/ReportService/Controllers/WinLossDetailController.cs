using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (WinlossDetail Read)
    public class WinLossDetailController : BaseReportController
    {
        private readonly IWinLossDetailService winLossDetailService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailController(
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
            // TODO: ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());
            var canViewExtraPT = true; // Placeholder
            var noteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);
            return Ok(new { canViewExtraPT, noteSabaOthers });
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
        public IActionResult ExportToExcel([FromBody] ExportWinLossDetailRequest request)
        {
            // TODO: Implement Excel export functionality
            // IWinLossDetailReportBuilder builder;
            // request.Viewlevel++;
            // var winlossDetail = GetDataFromDB(request);
            // ... complex logic ...
            // var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            // var memoryStream = builder.Build(formatedProducts);
            // if (memoryStream != null)
            // {
            //     return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            // }

            return Ok(new { message = "Excel export not yet implemented", fileName = request.FileName });
        }

        [HttpPost]
        [Route("GetExtraPT")]
        public IActionResult GetExtraPT([FromBody] ViewExtraPTRequest request)
        {
            request.UserId = currentSession.UserId; // TODO: Use correct property
            request.ParentGroupName = currentSession.ParentGroupName;
            var extraPTs = winLossDetailService.GetExtraPT(request);

            return Ok(extraPTs);
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
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }

        [HttpPost]
        [Route("GetCashFee")]
        public IActionResult GetCashFee([FromBody] DrillDownCashFeeRequest request)
        {
            var cashFee = winLossDetailService.DrillDownCashFee(request);
            return Ok(cashFee);
        }

        [HttpPost]
        [Route("ExportCashFeeToExcel")]
        public IActionResult ExportCashFeeToExcel([FromBody] ExportCashFeeRequest request)
        {
            // TODO: Implement Excel export functionality
            // ICashFeeBuilder builder;
            // var cashFee = winLossDetailService.DrillDownCashFee(request);
            // builder = new CashFeeBuilder(cashFee, request.FromDate, request.ToDate, request.UserName);
            // var memoryStream = builder.Build(string.Empty, request.UserName);
            // if (memoryStream != null)
            // {
            //     return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            // }

            return Ok(new { message = "Cash fee Excel export not yet implemented", fileName = request.FileName });
        }
    }

    // TODO: Define proper interfaces and services
    public partial interface IWinLossDetailService
    {
        object GetExtraPT(ViewExtraPTRequest request);
        object SearchWinLossDetail(SearchWinLossDetailRequest request);
        object DrillDownWinLossDetail(DrillDownWinLossDetailRequest request);
        object DrillDownCashFee(DrillDownCashFeeRequest request);
    }

    public partial class WinLossDetailService : IWinLossDetailService
    {
        public object GetExtraPT(ViewExtraPTRequest request)
        {
            // TODO: Implement actual service logic
            return new[] { new { id = 1, amount = 100.0m } };
        }

        public object SearchWinLossDetail(SearchWinLossDetailRequest request)
        {
            // TODO: Implement actual service logic
            return new { rows = "search results", request = request };
        }

        public object DrillDownWinLossDetail(DrillDownWinLossDetailRequest request)
        {
            // TODO: Implement actual service logic
            return new { rows = "drill down results", request = request };
        }

        public object DrillDownCashFee(DrillDownCashFeeRequest request)
        {
            // TODO: Implement actual service logic
            return new { cashFee = 50.0m, request = request };
        }
    }

    // TODO: Define proper request/response models
    public partial class WinLossDetailRequest
    {
        public int UserId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public int Viewlevel { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public string ProductIds { get; set; } = string.Empty;
        public string ResourceIds { get; set; } = string.Empty;

        public bool IsSearch()
        {
            // TODO: Implement actual logic
            return true;
        }
    }

    public class ViewExtraPTRequest
    {
        public int UserId { get; set; }
        public string ParentGroupName { get; set; } = string.Empty;
    }

    public class SearchWinLossDetailRequest
    {
        public SearchWinLossDetailRequest(WinLossDetailRequest request, object toolTipsProducts)
        {
            Request = request;
            ToolTipsProducts = toolTipsProducts;
        }

        public WinLossDetailRequest Request { get; }
        public object ToolTipsProducts { get; }
    }

    public class DrillDownWinLossDetailRequest
    {
        public DrillDownWinLossDetailRequest(WinLossDetailRequest request, object toolTipsProducts)
        {
            Request = request;
            ToolTipsProducts = toolTipsProducts;
        }

        public WinLossDetailRequest Request { get; }
        public object ToolTipsProducts { get; }
    }

    public class DrillDownCashFeeRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string UserName { get; set; } = string.Empty;
    }

    public class ExportWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{Viewlevel.DisplayRoleName()}WinLossDetail_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}.xls";
    }

    public class ExportCashFeeRequest : DrillDownCashFeeRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"CASH_SERVICE_BILLING_{FromDate.ToString(DateFormat)}_{ToDate.ToString(DateFormat)}.xls";
    }
}