namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.PokerGamesWinLossDetail;
    using Requests.PokerGamesWinLossDetail;
    using Service.Impl.Report;
    using Service.Interface.Report;

    [ExternalAccountAuthenticate(Function = eFunctions.PokerGamesWinLossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class PokerGamesWinLossDetailController : BaseController
    {
        private readonly PokerGamesWinLossDetailService reportService;

        public PokerGamesWinLossDetailController(
            ICurrentSession currentSession,
            PokerGamesWinLossDetailService reportService)
        {
            this.reportService = reportService ?? new PokerGamesWinLossDetailServiceImpl();
        }

        public PokerGamesWinLossDetailController() : this(null, null)
        {
        }

        public ActionResult Index() => View();

        [AntiXSRFFilter]
        public ActionResult GetData(PokerGamesWinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportPokerGamesWinLossDetailToExcel(ExportPokerGamesWinLossDetailRequest request)
        {
            IPokerGamesWinLossDetailReportBuilder builder;
            request.viewlevel++;
            var winlossDetail = GetDataFromDB(request);
            request.viewlevel--;
            switch (request.viewlevel)
            {
                case Role.Member:
                    builder = new MemberPokerGamesWinLossDetailReportBuilder(
                        (List<PokerGamesWinLossDetailAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                        request.viewlevel,
                        request.fromdate,
                        request.todate);

                    break;
                case Role.Agent:
                    builder = new AgentPokerGamesWinLossDetailReportBuilder(
                        (List<PokerGamesWinLossDetailMAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                        request.viewlevel,
                        request.fromdate,
                        request.todate);

                    break;
                default:
                    builder = new PokerGamesWinLossDetailReportBuilder(
                        (List<PokerGamesWinLossDetail>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                        request.viewlevel,
                        request.fromdate,
                        request.todate);

                    break;
            }

            var memoryStream = builder.Build();

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }

        private dynamic GetDataFromDB(PokerGamesWinLossDetailRequest request)
        {
            return request.IsSearch()
                ? reportService.SearchPokerGamesWinLossDetail(new SearchPokerGamesWinLossDetailRequest(request))
                : reportService.DrillDownPokerGamesWinLossDetail(new DrillDownPokerGamesWinLossDetailRequest(request));
        }
    }
}