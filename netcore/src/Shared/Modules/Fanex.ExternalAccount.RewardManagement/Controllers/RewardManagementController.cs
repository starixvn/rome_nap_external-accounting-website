namespace Fanex.ExternalAccount.RewardManagement.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using System.Web;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.RewardManagement.Services;
    using Fanex.ExternalAccount.RewardManagement.ViewModels;
    using Fanex.ExternalAccount.RewardManagement.Requests;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Newtonsoft.Json;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.RewardDetail, Policy = AuthenticateAction.AuthenticationType.Read)]
    public class RewardManagementController : BaseController
    {
        // RewardManagement
        private readonly IRewardManagementService service;
        public RewardManagementController(IRewardManagementService service, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.service = service;
        }
        
        public IActionResult Index()
        {
            var programs = service.GetPrograms();
            var viewModel = new RewardManagementViewModel(programs);
            return PartialViewBase(viewModel);
        }

        public IActionResult GetPrograms()
        {
            var programs = service.GetPrograms();
            return Json(programs);
        }

        public IActionResult GetReport(RewardReportRequest request)
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
            var fileName = $"RewardDetail_{request.FromDate.ToString("MMddyyyy")}_{request.ToDate.ToString("MMddyyyy")}.xls";

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