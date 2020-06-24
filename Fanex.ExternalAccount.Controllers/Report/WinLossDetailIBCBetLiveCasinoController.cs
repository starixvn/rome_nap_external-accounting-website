namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailIBCBetLiveCasinoReport;
    using Fanex.ExternalAccount.Utilities;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System.Collections.Generic;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.WinLossDetailIBCBetLiveCasino,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailIBCBetLiveCasinoController : BaseReportController
    {
        private readonly IWinLossDetailIBCBetLiveCasinoService winLossDetailIBCBetLiveCasinoService;

        public WinLossDetailIBCBetLiveCasinoController(
            IWinLossDetailIBCBetLiveCasinoService winLossDetailIBCBetLiveCasinoService,
            ICurrentSession currentSession,
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
        {
            if (!CanViewWinLossDetailForIBCBetLiveCasino())
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
                            request.FromDate,
                            request.ToDate);

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
            var searchRequest = new SearchWinLossDetailIBCBetLiveCasinoRequest(request);
            var winLossDetailIBCBetLiveCasino = winLossDetailIBCBetLiveCasinoService
                .SearchWinLossDetailIBCBetLiveCasino(searchRequest);

            return winLossDetailIBCBetLiveCasino;
        }
    }

    public sealed class ExportWinLossDetailIBCBetLiveCasinoRequest : WinLossDetailIBCBetLiveCasinoRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string GetFileName()
        {
            string fromDate = FromDate.ToString(DateFormat);
            string toDate = ToDate.ToString(DateFormat);
            string fileName = $"WinLossDetailForIBCBetLiveCasino_{fromDate}_{toDate}.xls";

            return fileName;
        }
    }
}