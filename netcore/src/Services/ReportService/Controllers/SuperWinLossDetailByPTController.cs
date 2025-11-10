using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.Controllers.Report
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize] // TODO: Update with proper authorization policy (SuperWinLossDetailByPT Read)
    public class SuperWinLossDetailByPTController : BaseReportController
    {
        private readonly ISuperWinLossDetailByPTService services;
        private readonly IProductDetailService productDetailService;

        public SuperWinLossDetailByPTController(
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
            // TODO: var noteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);
            var noteSabaOthers = new { message = "Note Saba Others" }; // Placeholder
            return Ok(new { noteSabaOthers });
        }

        [HttpPost]
        [Route("GetData")]
        public IActionResult GetData([FromBody] SuperWinLossDetailByPTRequest request)
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
                request.PageSize,
                request.PageIndex,
                request.UserId);

            var viewModel = services.GetSuperWinLossDetailByPT(param);
            return Ok(viewModel);
        }

        [HttpPost]
        [Route("ExportToExcel")]
        public IActionResult ExportToExcel([FromBody] ExportSuperWinLossDetailByPTRequest request)
        {
            // TODO: Implement Excel export functionality
            // var winlossDetail = services.GetSuperWinLossDetailByPT(new SuperWinLossDetailByPTParam(...));
            // ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByPTReportBuilder(...);
            // var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            // var memoryStream = builder.Build(formatedProducts);
            // if (memoryStream != null)
            // {
            //     return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            // }

            return Ok(new { message = "Excel export not yet implemented", fileName = request.FileName });
        }
    }

    // TODO: Define proper interfaces and services
    public partial interface ISuperWinLossDetailByPTService
    {
        object GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param);
    }

    public partial class SuperWinLossDetailByPTService : ISuperWinLossDetailByPTService
    {
        public object GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param)
        {
            // TODO: Implement actual service logic
            return new { winLossByUsernames = new[] { new { username = "user1", amount = 100.0m } }, grandTotal = new { total = 100.0m } };
        }
    }

    public interface IProductDetailService
    {
        object GetProductDetailBelongSabaOtherSports(string groupName);
    }

    public class ProductDetailService : IProductDetailService
    {
        public object GetProductDetailBelongSabaOtherSports(string groupName)
        {
            // TODO: Implement actual service logic
            return new { note = "Saba other sports details" };
        }
    }

    // TODO: Define proper request/response models
    public partial class SuperWinLossDetailByPTRequest
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public string ProductIds { get; set; } = string.Empty;
        public string ResourceIds { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageIndex { get; set; }
        public int UserId { get; set; }
    }

    public class ExportSuperWinLossDetailByPTRequest
    {
        public int GroupId { get; set; }
        public string GroupName { get; set; } = string.Empty;
        public DateTime Fromdate { get; set; }
        public DateTime Todate { get; set; }
        public string ProductIds { get; set; } = string.Empty;
        public string ResourceIds { get; set; } = string.Empty;
        public int UserId { get; set; }
        public string FileName { get; set; } = string.Empty;
    }

    public partial class SuperWinLossDetailByPTParam
    {
        public SuperWinLossDetailByPTParam(
            int groupId, string groupName, DateTime fromdate, DateTime todate,
            string productIds, string resourceIds, object tooltipsLiveCasino,
            object tooltipsRNGCasino, int pageSize, int pageIndex, int userId)
        {
            GroupId = groupId;
            GroupName = groupName;
            Fromdate = fromdate;
            Todate = todate;
            ProductIds = productIds;
            ResourceIds = resourceIds;
            TooltipsLiveCasino = tooltipsLiveCasino;
            TooltipsRNGCasino = tooltipsRNGCasino;
            PageSize = pageSize;
            PageIndex = pageIndex;
            UserId = userId;
        }

        public int GroupId { get; }
        public string GroupName { get; }
        public DateTime Fromdate { get; }
        public DateTime Todate { get; }
        public string ProductIds { get; }
        public string ResourceIds { get; }
        public object TooltipsLiveCasino { get; }
        public object TooltipsRNGCasino { get; }
        public int PageSize { get; }
        public int PageIndex { get; }
        public int UserId { get; }
    }
}