namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;
        private readonly dynamic ToolTipsProducts;

        public WinLossDetailController(
            ICurrentSession currentSession,
            WinLossDetailService winLossDetailService,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
            : base(currentSession, reportService, groupConfigurationService)
        {
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();
            ToolTipsProducts = new
            {
                showAllbet = this.groupConfigurationService.IsShowByProductAndGroup(ProductConstants.AllBet, this.currentSession.CurrencyGroup),
            };
        }

        public WinLossDetailController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
            => View();

        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
        {
            IWinLossDetailReportBuilder builder;
            request.viewlevel++;
            var winlossDetail = GetDataFromDB(request);
            request.viewlevel--;
            switch (request.viewlevel)
            {
                case Role.Member:
                    {
                        builder = new MemberWinLossDetailReportBuilder(
                            (List<WinLossDetailAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailReportBuilder(
                            (List<WinLossDetailMAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailReportBuilder(
                            (List<WinLossDetail>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
            }

            var formatedProducts = request.GetProductNames(GetProductList());
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, ToolTipsProducts))
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, ToolTipsProducts));
        }
    }

    public class ExportWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}WinLossDetail_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}