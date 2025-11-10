using Microsoft.AspNetCore.Mvc;
using Fanex.ExternalAccount.ReportsAndStatements.Utilities;
using Fanex.ExternalAccount.ReportsAndStatements.Services.Reports;

namespace Fanex.ExternalAccount.ReportsAndStatements.Controllers.Reports
{
    public class WinLossDetailController : BaseReportController
    {
        private readonly IWinLossDetailService _winLossDetailService;
        private readonly IProductDetailService _productDetailService;

        public WinLossDetailController(
            CurrentSession currentSession,
            IWinLossDetailService winLossDetailService,
            IProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            _winLossDetailService = winLossDetailService ?? new WinLossDetailService();
            _productDetailService = productDetailService ?? new ProductDetailService();
        }

        public IActionResult Index()
        {
            ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());
            ViewBag.NoteSabaOthers = _productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetExtraPT(ViewExtraPTRequest request)
        {
            request.UserId = currentSession.CustId;
            request.ParentGroupName = currentSession.ParentGroupName;
            var extraPTs = _winLossDetailService.GetExtraPT(request);

            return Json(extraPTs);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetCashFee(DrillDownCashFeeRequest request)
        {
            var cashFee = _winLossDetailService.DrillDownCashFee(request);
            return Json(cashFee);
        }

        public IActionResult ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
        {
            // TODO: Implement Excel export logic
            return File(new byte[0], "application/vnd.ms-excel", request.FileName);
        }

        public IActionResult ExportCashFeeToExcel(ExportCashFeeRequest request)
        {
            // TODO: Implement Excel export logic
            return File(new byte[0], "application/vnd.ms-excel", request.FileName);
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                rngCasino = reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId)
            };

            return request.IsSearch()
                ? _winLossDetailService.SearchWinLossDetail(new SearchWinLossDetailRequest(request, toolTipsProducts))
                : _winLossDetailService.DrillDownWinLossDetail(new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }
    }

    // Placeholder classes and interfaces - need to be properly implemented
    public interface IWinLossDetailService
    {
        dynamic GetExtraPT(ViewExtraPTRequest request);
        dynamic DrillDownCashFee(DrillDownCashFeeRequest request);
        dynamic SearchWinLossDetail(SearchWinLossDetailRequest request);
        dynamic DrillDownWinLossDetail(DrillDownWinLossDetailRequest request);
    }

    public class WinLossDetailService : IWinLossDetailService
    {
        public dynamic GetExtraPT(ViewExtraPTRequest request) => new { };
        public dynamic DrillDownCashFee(DrillDownCashFeeRequest request) => new { };
        public dynamic SearchWinLossDetail(SearchWinLossDetailRequest request) => new { rows = new object[] { } };
        public dynamic DrillDownWinLossDetail(DrillDownWinLossDetailRequest request) => new { rows = new object[] { } };
    }

    public interface IProductDetailService
    {
        string GetProductDetailBelongSabaOtherSports(string groupName);
    }

    public class ProductDetailService : IProductDetailService
    {
        public string GetProductDetailBelongSabaOtherSports(string groupName) => string.Empty;
    }

    public class WinLossDetailRequest
    {
        public int UserId { get; set; }
        public string GroupName { get; set; }
        public int GroupId { get; set; }
        public string ProductIds { get; set; }
        public string ResourceIds { get; set; }
        public int Viewlevel { get; set; }
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public string FileName { get; set; }

        public bool IsSearch() => true; // Placeholder
    }

    public class ViewExtraPTRequest
    {
        public int UserId { get; set; }
        public string ParentGroupName { get; set; }
    }

    public class DrillDownCashFeeRequest { }

    public class ExportWinLossDetailRequest : WinLossDetailRequest { }

    public class ExportCashFeeRequest
    {
        public string FileName { get; set; } = string.Empty;
    }

    public class SearchWinLossDetailRequest
    {
        public SearchWinLossDetailRequest(WinLossDetailRequest request, object toolTips) { }
    }

    public class DrillDownWinLossDetailRequest
    {
        public DrillDownWinLossDetailRequest(WinLossDetailRequest request, object toolTips) { }
    }
}
