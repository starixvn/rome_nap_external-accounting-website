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

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailByPTBaseCurrency,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SuperWinLossDetailMCByPTController : BaseReportController
    {
        private readonly SuperWinLossDetailByPTService _superWinLossDetailByPTService;
        private readonly IProductDetailService _productDetailService;

        public SuperWinLossDetailMCByPTController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService,
            SuperWinLossDetailByPTService superWinLossDetailByPTService,
            IProductDetailService productDetailService,
            IReportService _reportService)
            : base(_reportService, currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _superWinLossDetailByPTService = superWinLossDetailByPTService;
            _productDetailService = productDetailService;
        }

        public IActionResult Index()
        {
            ViewBag.NoteSabaOthers = _productDetailService.GetProductDetailBelongSabaOtherSports(CurrentSession.ParentGroupName);

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
                0,
                0,
                request.UserId)
            {
                BaseCurrency = request.BaseCurrency,
                BaseCurrencyName = request.BaseCurrencyName,
                Currencies = request.Currencies
            };

            var viewModel = _superWinLossDetailByPTService.GetSuperWinLossMCDetailByPT(param);
            return Ok(viewModel);
        }

        public IActionResult ExportWinLossDetailToExcel(ExportSuperWinLossDetailByPTRequest request)
        {
            var winlossDetail = _superWinLossDetailByPTService.GetSuperWinLossMCDetailByPT(new SuperWinLossDetailByPTParam(
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
                request.UserId)
            {
                BaseCurrency = request.BaseCurrency,
                BaseCurrencyName = request.BaseCurrencyName,
                Currencies = request.Currencies
            });

            ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByMCPTReportBuilder(
                winlossDetail.WinLossByUsernames == null ? new List<SuperWinLossDetailMCByUsernameModel>() : winlossDetail.WinLossByUsernames.ToList(),
                winlossDetail.TotalByCurrencies == null ? new List<SuperWinLossDetailMCByProductDTO>() : winlossDetail.TotalByCurrencies.ToList(),
                request.BaseCurrencyName,
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