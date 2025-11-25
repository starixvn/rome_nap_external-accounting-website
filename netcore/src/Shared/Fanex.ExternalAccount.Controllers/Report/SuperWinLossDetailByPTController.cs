namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT;
    using Requests;
    using Service.Interface.Report;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailByPT,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SuperWinLossDetailByPTController : BaseReportController
    {
        private readonly ISuperWinLossDetailByPTService services;
        private readonly IProductDetailService productDetailService;

        public SuperWinLossDetailByPTController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            IAuthorizationService authorizationService,
            ISuperWinLossDetailByPTService services,
            IProductDetailService productDetailService,
            IReportService reportService)
            : base(reportService, currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.services = services;
            this.productDetailService = productDetailService;
        }

        public IActionResult Index()
        {
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(CurrentSession.ParentGroupName);

            return View();
        }
            

        [ValidateAntiForgeryToken]
        public IActionResult GetData(SuperWinLossDetailByPTRequest request)
        {
            var param = new SuperWinLossDetailByPTParam(
                request.GroupId,
                request.GroupName,
                request.Fromdate,
                request.Todate,
                request.ProductIds,
                request.ResourceIds,
                _reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                _reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId),
                request.PageSize,
                request.PageIndex,
                request.UserId);

            var viewModel = services.GetSuperWinLossDetailByPT(param);
            return Ok(viewModel);
        }

        public IActionResult ExportWinLossDetailToExcel(ExportSuperWinLossDetailByPTRequest request)
        {
            var winlossDetail = services.GetSuperWinLossDetailByPT(new SuperWinLossDetailByPTParam(
                request.GroupId,
                request.GroupName,
                request.Fromdate,
                request.Todate,
                request.ProductIds,
                request.ResourceIds,
                _reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                _reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId),
                0,
                0,
                request.UserId));

            ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByPTReportBuilder(
                winlossDetail.WinLossByUsernames == null ? new List<SuperWinLossDetailByUsernameModel>() : winlossDetail.WinLossByUsernames.ToList(),
                winlossDetail.GrandTotal ?? new SuperWinLossDetailByProductDTO(),
                request.Fromdate,
                request.Todate);

            var formatedProducts = _reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                memoryStream.Position = 0;
                return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            }
            
            return BadRequest("Failed to generate Excel file");
        }
    }
}