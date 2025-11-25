namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;
    using Requests;
    using Service.Interface.Report;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;
        private readonly IProductDetailService productDetailService;

        public WinLossDetailController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService,
            WinLossDetailService winLossDetailService,
            IProductDetailService productDetailService,
            IReportService reportService)
            : base(reportService, currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.winLossDetailService = winLossDetailService;
            this.productDetailService = productDetailService;
        }

        public IActionResult Index()
        {
            ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(CurrentSession.ParentGroupName);
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Ok(viewModel);
        }

        public IActionResult ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
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
                        builder = new MemberWinLossDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetailAg>() : (List<WinLossDetailAg>)winlossDetailData,
                            Role.Agent,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetailMAg>() : (List<WinLossDetailMAg>)winlossDetailData,
                            Role.Master,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Master:
                    {
                        builder = new WinLossDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetail>() : (List<WinLossDetail>)winlossDetailData,
                            Role.Super,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailExtraReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetailExtra>() : (List<WinLossDetailExtra>)winlossDetailData,
                            Role.Group,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
            }

            var formatedProducts = _reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                memoryStream.Position = 0;
                return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            }
            
            return BadRequest("Failed to generate Excel file");
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetExtraPT(ViewExtraPTRequest request)
        {
            request.UserId = CurrentSession.CustId;
            request.ParentGroupName = CurrentSession.ParentGroupName;
            var extraPTs = winLossDetailService.GetExtraPT(request);

            return Ok(extraPTs);
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = _reportService.BuildTooltipsForLiveCasino(request.UserId, request.GroupName, request.GroupId),
                rngCasino = _reportService.BuildTooltipsForRNGCasino(request.UserId, request.GroupName, request.GroupId)
            };

            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, toolTipsProducts))
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetCashFee(DrillDownCashFeeRequest request)
        {
            var cashFee = winLossDetailService.DrillDownCashFee(request);

            return Ok(cashFee);
        }

        public IActionResult ExportCashFeeToExcel(ExportCashFeeRequest request)
        {
            ICashFeeBuilder builder;
          
            var cashFee = winLossDetailService.DrillDownCashFee(request); 
           
            builder = new CashFeeBuilder(
                            cashFee,
                            request.FromDate,
                            request.ToDate,
                            request.UserName);


            var memoryStream = builder.Build(string.Empty, request.UserName);

            if (memoryStream != null)
            {
                memoryStream.Position = 0;
                return File(memoryStream, "application/vnd.ms-excel", request.FileName);
            }
            
            return BadRequest("Failed to generate Excel file");
        }

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