namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Service.Products;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;

    [ExternalAccountAuthenticate(Function = eFunctions.WinLossDetailsBaseCurrency,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailMCController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;
        private readonly ProductDetailService productDetailService;

        public WinLossDetailMCController(
            CurrentSession currentSession,
            WinLossDetailService winLossDetailService,
            ProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public WinLossDetailMCController() : this(null!, null!, null!, null!)
        {
        }

        public IActionResult Index()
        {
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);

            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Ok(viewModel);
        }

        public IActionResult ExportWinLossDetailToExcel(ExportWinLossMCDetailRequest request)
        {
            IWinLossDetailReportBuilder builder;
            request.Viewlevel++;
            var winlossDetail = GetDataFromDB(request);
            var winlossDetailData = winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null);
            var isWinlossDetailNoData = winlossDetailData is string;
            request.Viewlevel--;
            switch (request.Viewlevel)
            {
                case Role.Member:
                    {
                        builder = new MemberWinLossByCurrencyDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossByCurrencyDetailAg>() : ((WinLossByCurrencyAg)winlossDetailData).WinLossDetails,
                            ((WinLossByCurrencyAg)winlossDetailData).WinLossTotals,
                            Role.Agent,
                            request.Fromdate,
                            request.Todate,
                            request.BaseCurrencyName);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossByCurrencyDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossByCurrencyDetailMAg>() : ((WinLossByCurrencyMAg)winlossDetailData).WinLossDetails,
                            ((WinLossByCurrencyMAg)winlossDetailData).WinLossTotals,
                            Role.Master,
                            request.Fromdate,
                            request.Todate,
                            request.BaseCurrencyName);

                        break;
                    }
                case Role.Master:
                    {
                        builder = new WinLossByCurrencyDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossByCurrencyDetail>() : ((WinLossByCurrency)winlossDetailData).WinLossDetails,
                            ((WinLossByCurrency)winlossDetailData).WinLossTotals,
                            Role.Super,
                            request.Fromdate,
                            request.Todate,
                            request.BaseCurrencyName);  
                        break;
                    }
                case Role.Super:
                    {
                        builder = new WinLossByCurrencyDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossByCurrencyDetail>() : ((WinLossByCurrency)winlossDetailData).WinLossDetails,
                            ((WinLossByCurrency)winlossDetailData).WinLossTotals,
                            Role.Group,
                            request.Fromdate,
                            request.Todate,
                            request.BaseCurrencyName);

                        break;
                    }
                default:
                    throw new ArgumentException($"Invalid RoleId = {request.Viewlevel}");
            }

            var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                memoryStream.Position = 0;
                return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            }
            
            return BadRequest("Failed to generate Excel file");
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
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

    public class ExportWinLossMCDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{Viewlevel.DisplayRoleName()}WinLossDetail_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}_in_{(BaseCurrency == 0 ? "customer_currency" : BaseCurrencyName)}.xls";
    }
}