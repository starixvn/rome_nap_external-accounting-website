namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Controllers
{
    using System.Web.Mvc;
    using System.Web;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Models;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Requests;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.Services;
    using Fanex.ExternalAccount.WhiteLabelWinLossReports.ViewModels;
    using Newtonsoft.Json;

    [ExternalAccountAuthenticate(Function = eFunctions.WhiteLabelSettlementReport, Policy = AuthenticateAction.AuthenticationType.Read)]
    public class WhiteLabelReportController : BaseController
    {
        private readonly IWhiteLabelReportService service;
        
        public WhiteLabelReportController() : base()
        {
            service = new WhiteLabelReportService();
        }

        public ActionResult Index()
        {
            var viewModel = new WhiteLabelReportViewModel(service.GetWhiteLabels(CurrentSession.CurrencyGroup));

            return PartialViewBase(viewModel);
        }

        public ActionResult GetSettlement(WhiteLabelReportQuery message)
        {
            message.GroupName = CurrentSession.CurrencyGroup;
            message.Username = HttpUtility.HtmlEncode(message.Username);
            
            return this.JsonExt(service.GetSettlement(message), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public void ExportSettlement(WhiteLabelReportQuery message)
        {
            message.GroupName = CurrentSession.CurrencyGroup;
            message.Username = HttpUtility.HtmlEncode(message.Username);
            var reportData = service.GetSettlement(message);
            var fileName = $"WhiteLabel_SettlementReport_{message.FromDate.ToString("MMyyyy")}_{message.ToDate.ToString("MMyyyy")}.xls";

            using (var excel = new WhiteLabelSettlementExcel
            {
                FromDate = message.FromDate,
                ToDate = message.ToDate,
                ReportData = reportData
            })
            {
                using (var memoryString = excel.BuildMemoryStream())
                {
                    ExportToExcel(memoryString, fileName);
                }
            }
        }

        public ActionResult GetDailyWinLoss(WhiteLabelReportQuery message)
        {
            message.GroupName = CurrentSession.CurrencyGroup;
            message.Username = HttpUtility.HtmlEncode(message.Username);
            return this.JsonExt(service.GetDailyWinLoss(message), new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
        }

        public void ExportDailyWinLoss(WhiteLabelReportQuery message)
        {
            message.GroupName = CurrentSession.CurrencyGroup;
            message.Username = HttpUtility.HtmlEncode(message.Username);
            var reportData = service.GetDailyWinLoss(message);
            var fileName = $"WhiteLabel_DailyWinLossReport_{message.FromDate.ToString("MMddyyyy")}_{message.ToDate.ToString("MMddyyyy")}.xls";

            using (var excel = new WhiteLabelDailyWinLossExcel
            {
                FromDate = message.FromDate,
                ToDate = message.ToDate,
                ReportData = reportData
            })
            {
                using (var memoryString = excel.BuildMemoryStream())
                {
                    ExportToExcel(memoryString, fileName);
                }
            }
        }
    }
}
