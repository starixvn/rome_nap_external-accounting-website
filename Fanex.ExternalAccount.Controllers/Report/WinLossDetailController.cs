namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;

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

        [AntiXSRFFilter]
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
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetailMAg>() : (List<WinLossDetailMAg>)winlossDetailData,
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Master:
                    {
                        builder = new WinLossDetailReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetail>() : (List<WinLossDetail>)winlossDetailData,
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailExtraReportBuilder(
                            isWinlossDetailNoData ? new List<WinLossDetailExtra>() : (List<WinLossDetailExtra>)winlossDetailData,
                            request.Viewlevel,
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

        [AntiXSRFFilter]
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

    }

    public class ExportWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{Viewlevel.DisplayRoleName()}WinLossDetail_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}.xls";
    }
}