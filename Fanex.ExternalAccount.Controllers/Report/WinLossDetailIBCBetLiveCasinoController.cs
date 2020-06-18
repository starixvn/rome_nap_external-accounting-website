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
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
            : base(currentSession, reportService, groupConfigurationService)
        {
            winLossDetailIBCBetLiveCasinoService = new WinLossDetailIBCBetLiveCasinoService();
        }

        public WinLossDetailIBCBetLiveCasinoController() : this(null, null, null)
        {
        }

        public ActionResult Index()
        {
            bool isIBCINDO = IsIBCINDO();
            if (!isIBCINDO)
            {
                return PartialView("NoPermission");
            }

            return View();
        }

        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailIBCBetLiveCasinoRequest request)
        {
            var winLossDetailIBCBetLiveCasino = GetDataFromDB(request);

            return Json(winLossDetailIBCBetLiveCasino);
        }

        public void ExportWinLossDetailIBCBetLiveCasinoToExcel(ExportWinLossDetailIBCBetLiveCasinoRequest request)
        {
            var winlossDetailIBCBetLiveCasino = GetDataFromDB(request);
            var winlossDetailIBCBetLiveCasinoList = (List<WinLossDetailIBCBetLiveCasino>)winlossDetailIBCBetLiveCasino
                .GetType()
                .GetProperty("rows")
                .GetValue(winlossDetailIBCBetLiveCasino, null);

            var builder = new WinLossDetailIBCBetLiveCasinoReportBuilder(
                            winlossDetailIBCBetLiveCasinoList,
                            request.fromdate,
                            request.todate);

            var productList = GetProductList();
            var productNames = request.GetProductNames(productList);

            var memoryStream = builder.Build(productNames);
            if (memoryStream != null)
            {
                string fileName = request.GetFileName();
                ExportToExcel(memoryStream, fileName);

                memoryStream.Dispose();
            }
        }

        private dynamic GetDataFromDB(WinLossDetailIBCBetLiveCasinoRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = BuildTooltipsForLiveCasino(),
            };

            var searchRequest = new SearchWinLossDetailIBCBetLiveCasinoRequest(request, toolTipsProducts)
            {
                ProductIds = $"{ProductConstants.GTCasino}",
            };
            var winLossDetailIBCBetLiveCasino = winLossDetailIBCBetLiveCasinoService
                .SearchWinLossDetailIBCBetLiveCasino(searchRequest);

            return winLossDetailIBCBetLiveCasino;
        }

        private string BuildTooltipsForLiveCasino()
        {
            var liveCasinos = new List<GroupConfigurationModel>
            {
                new GroupConfigurationModel
                {
                    ProductId = ProductConstants.GTCasino,
                    ProductName = "IBCBet Live Casino"
                }
            };

            var offeredLiveCasinos = liveCasinos
                .Where(liveCasino => groupConfigurationService.IsShowByProductAndGroup(liveCasino.ProductId, currentSession.CurrencyGroup));

            if (offeredLiveCasinos.Any())
            {
                var liveCasinoName = string.Join(", ", offeredLiveCasinos.Select(offeredLiveCasino => offeredLiveCasino.ProductName));

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }
    }

    public sealed class ExportWinLossDetailIBCBetLiveCasinoRequest : WinLossDetailIBCBetLiveCasinoRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string GetFileName()
        {
            string displayRoleName = viewlevel.DisplayRoleName();
            string fromDate = fromdate.ToString(DateFormat);
            string toDate = todate.ToString(DateFormat);
            string fileName = $"{displayRoleName}WinLossDetailForIBCBetLiveCasino_{fromDate}_{toDate}.xls";

            return fileName;
        }
    }
}