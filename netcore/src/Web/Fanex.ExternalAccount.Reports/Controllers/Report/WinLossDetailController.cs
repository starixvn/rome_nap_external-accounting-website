namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Service.Interface;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Controllers.Requests;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;
        private readonly ProductDetailService productDetailService;

        public WinLossDetailController(
            CurrentSession currentSession,
            WinLossDetailService winLossDetailService,
            ProductDetailService productDetailService,
            IReportService reportService)
            : base(currentSession, reportService)
        {
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public WinLossDetailController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
        {
            ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());
            ViewBag.NoteSabaOthers = productDetailService.GetProductDetailBelongSabaOtherSports(currentSession.ParentGroupName);
            return View();
        }

        //[ValidateAntiForgeryToken]
        public ActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
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

            var formatedProducts = reportService.ConcatProductName(request.ProductIds, request.ResourceIds, request.GroupName);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }

        //[ValidateAntiForgeryToken]
        public ActionResult GetExtraPT(ViewExtraPTRequest request)
        {
            request.UserId = currentSession.CustId;
            request.ParentGroupName = currentSession.ParentGroupName;
            var extraPTs = winLossDetailService.GetExtraPT(request);

            return Json(extraPTs);
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
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }

        //[ValidateAntiForgeryToken]
        public ActionResult GetCashFee(DrillDownCashFeeRequest request)
        {
            var cashFee = winLossDetailService.DrillDownCashFee(request);

            return Json(cashFee);
        }

        public void ExportCashFeeToExcel(ExportCashFeeRequest request)
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
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
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