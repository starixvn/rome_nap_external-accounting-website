namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT;
    using Requests;
    using Service.Interface.Report;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailByPT,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SuperWinLossDetailByPTController : BaseReportController
    {
        private readonly SuperWinLossDetailByPTService services;
        private readonly ProductDetailService productDetailService;

        public SuperWinLossDetailByPTController(
            CurrentSession currentSession,
            SuperWinLossDetailByPTService services,
            ProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.services = services ?? new SuperWinLossDetailByPTServiceImpl();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public SuperWinLossDetailByPTController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
        {
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);

            return View();
        }
            

        [ValidateAntiForgeryToken]
        public ActionResult GetData(SuperWinLossDetailByPTRequest request)
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
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportSuperWinLossDetailByPTRequest request)
        {
            var winlossDetail = services.GetSuperWinLossDetailByPT(new SuperWinLossDetailByPTParam(
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
                request.UserId));

            ISuperWinLossDetailByPTReportBuilder builder = new SuperWinLossDetailByPTReportBuilder(
                winlossDetail.WinLossByUsernames == null ? new List<SuperWinLossDetailByUsernameModel>() : winlossDetail.WinLossByUsernames.ToList(),
                winlossDetail.GrandTotal ?? new SuperWinLossDetailByProductDTO(),
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