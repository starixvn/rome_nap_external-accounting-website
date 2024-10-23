namespace Fanex.ExternalAccount.RewardManagement.Controllers
{
    using System.Web.Mvc;
    using System.Web;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.RewardManagement.Services;
    using Fanex.ExternalAccount.RewardManagement.ViewModels;
    using Fanex.ExternalAccount.RewardManagement.Requests;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Newtonsoft.Json;

    [ExternalAccountAuthenticate(Function = eFunctions.RewardDetail, Policy = AuthenticateAction.AuthenticationType.Read)]
    public class RewardManagementController : BaseController
    {
        // RewardManagement
        private readonly IRewardManagementService service;

        public RewardManagementController() : base()
        {
            service = new RewardManagementService();
        }

        public ActionResult Index()
        {
            var programs = service.GetPrograms();
            var viewModel = new RewardManagementViewModel(programs);
            return PartialViewBase(viewModel);
        }

        public ActionResult GetPrograms()
        {
            var programs = service.GetPrograms();
            return Json(programs, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetReport(RewardReportRequest request)
        {
            request.GroupName = CurrentSession.CurrencyGroup;
            request.Username = HttpUtility.HtmlEncode(request.Username);
            var result = service.GetReport(request);
            return this.JsonExt(result, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore });
        }
        
        public void ExportRewardDetail(RewardReportRequest request)
        {
            request.GroupName = CurrentSession.CurrencyGroup;
            request.Username = HttpUtility.HtmlEncode(request.Username);
            var reportData = service.GetReport(request);
            var fileName = $"RewardDetail_{request.FromDate.ToString("MMyyyy")}_{request.ToDate.ToString("MMyyyy")}.xls";

            using (var excel = new RewardDetailExcel
                   {
                       FromDate = request.FromDate,
                       ToDate = request.ToDate,
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