namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailIBCBetLiveCasinoReport;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Utilities.Reports.WinLossDetailIBCBetLiveCasino;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.WinLossDetailIBCBetLiveCasino,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailIBCBetLiveCasinoController : BaseReportController
    {
        private readonly IWinLossDetailIBCBetLiveCasinoService winLossDetailIBCBetLiveCasinoService;

        public WinLossDetailIBCBetLiveCasinoController(
            ICurrentSession currentSession,
            IWinLossDetailIBCBetLiveCasinoService winLossDetailIBCBetLiveCasinoService,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
            : base(currentSession, reportService, groupConfigurationService)
        {
            this.winLossDetailIBCBetLiveCasinoService = winLossDetailIBCBetLiveCasinoService ?? new WinLossDetailIBCBetLiveCasinoService();
        }

        public WinLossDetailIBCBetLiveCasinoController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
            => View();

        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailIBCBetLiveCasinoRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportWinLossDetailIBCBetLiveCasinoToExcel(ExportWinLossDetailIBCBetLiveCasinoRequest request)
        {
            IWinLossDetailIBCBetLiveCasinoReportBuilder builder;
            request.viewlevel++;
            var winlossDetailIBCBetLiveCasino = GetDataFromDB(request);
            request.viewlevel--;
            switch (request.viewlevel)
            {
                case Role.Member:
                    {
                        builder = new MemberWinLossDetailIBCBetLiveCasinoReportBuilder(
                            (List<WinLossDetailIBCBetLiveCasinoAg>)winlossDetailIBCBetLiveCasino.GetType().GetProperty("rows").GetValue(winlossDetailIBCBetLiveCasino, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailIBCBetLiveCasinoReportBuilder(
                            (List<WinLossDetailIBCBetLiveCasinoMAg>)winlossDetailIBCBetLiveCasino.GetType().GetProperty("rows").GetValue(winlossDetailIBCBetLiveCasino, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailIBCBetLiveCasinoReportBuilder(
                            (List<WinLossDetailIBCBetLiveCasino>)winlossDetailIBCBetLiveCasino.GetType().GetProperty("rows").GetValue(winlossDetailIBCBetLiveCasino, null),
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

        private dynamic GetDataFromDB(WinLossDetailIBCBetLiveCasinoRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = BuildTooltipsForLiveCasino()
            };

            return request.IsSearch()
                ? winLossDetailIBCBetLiveCasinoService.SearchWinLossDetailIBCBetLiveCasino(
                    new SearchWinLossDetailIBCBetLiveCasinoRequest(request, toolTipsProducts))
                : winLossDetailIBCBetLiveCasinoService.DrillDownWinLossDetailIBCBetLiveCasino(
                    new DrillDownWinLossDetailIBCBetLiveCasinoRequest(request, toolTipsProducts));
        }

        private string BuildTooltipsForLiveCasino()
        {
            var liveCasino = new List<GroupConfigurationModel>
            {
                new GroupConfigurationModel{ProductId=ProductConstants.AllBet,ProductName="AllBet"},
                new GroupConfigurationModel{ProductId=ProductConstants.SexyGaming,ProductName="Sexy Gaming"},
                new GroupConfigurationModel{ProductId=ProductConstants.SAGaming,ProductName="SA Gaming"},
                new GroupConfigurationModel{ProductId=ProductConstants.GTCasino,ProductName="IBCBet Live Casino"},
                new GroupConfigurationModel{ProductId=ProductConstants.BBin,ProductName="BBIN"},
                new GroupConfigurationModel{ProductId=ProductConstants.ION,ProductName="ION"}
            };

            var offeredLiveCasino = liveCasino.Where(
                i => this.groupConfigurationService.IsShowByProductAndGroup(i.ProductId, this.currentSession.CurrencyGroup));

            if (offeredLiveCasino != null && offeredLiveCasino.Any())
            {
                var liveCasinoName = string.Join(", ", offeredLiveCasino.Select(i => i.ProductName));

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }
    }

    public class ExportWinLossDetailIBCBetLiveCasinoRequest : WinLossDetailIBCBetLiveCasinoRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}WinLossDetailIBCBetLiveCasino_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}
