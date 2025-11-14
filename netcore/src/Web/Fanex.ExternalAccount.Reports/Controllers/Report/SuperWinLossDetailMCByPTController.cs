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

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailByPTBaseCurrency,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SuperWinLossDetailMCByPTController : BaseReportController
    {
        private readonly SuperWinLossDetailByPTService services;
        private readonly ProductDetailService productDetailService;

        public SuperWinLossDetailMCByPTController(
            CurrentSession currentSession,
            SuperWinLossDetailByPTService services,
            ProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.services = services ?? new SuperWinLossDetailByPTServiceImpl();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public SuperWinLossDetailMCByPTController() : this(null, null, null, null)
        {
        }

        public IActionResult Index()
        {
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);

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
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportSuperWinLossDetailByPTRequest request)
        {
            var winlossDetail = services.GetSuperWinLossMCDetailByPT(new SuperWinLossDetailByPTParam(
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
            });

            ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByMCPTReportBuilder(
                winlossDetail.WinLossByUsernames == null ? new List<SuperWinLossDetailMCByUsernameModel>() : winlossDetail.WinLossByUsernames.ToList(),
                winlossDetail.TotalByCurrencies == null ? new List<SuperWinLossDetailMCByProductDTO>() : winlossDetail.TotalByCurrencies.ToList(),
                request.BaseCurrencyName,
                request.Fromdate,
                request.Todate);

            var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }
    }
}