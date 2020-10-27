namespace Fanex.ExternalAccount.ProblemAccount.Controllers
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Models;
    using Fanex.ExternalAccount.ProblemAccount.Resources;
    using Fanex.ExternalAccount.ProblemAccount.Services;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class ProblemAccountController : BaseController
    {
        private readonly IProblemAccountService service;
        private readonly IProblemAccountExcel excel;

        public ProblemAccountController()
            : this(null, null)
        {
        }

        public ProblemAccountController(IProblemAccountService service, IProblemAccountExcel excel)
        {
            this.service = service ?? new ProblemAccountService();
            this.excel = excel ?? new ProblemAccountExcel();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ProblemAccountList, Policy = AuthenticationType.Read)]
        public ActionResult Index()
            => PartialViewBase(
                new IndexViewModel(AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.ProblemAccountList, AuthenticationType.Host)));

        [ExternalAccountAuthenticate(Function = eFunctions.ProblemAccountList, Policy = AuthenticationType.Read)]
        public ActionResult GetList(GetListViewModel viewModel)
            => Json(service.GetProblemAccounts(viewModel), JsonRequestBehavior.AllowGet);

        [ExternalAccountAuthenticate(Function = eFunctions.ProblemAccountList, Policy = AuthenticationType.Read)]
        public void ExportExcel(GetListViewModel viewModel)
        {
            const int allPageSize = int.MaxValue - 2;
            viewModel.PageIndex = 0;
            viewModel.PageSize = allPageSize;
            var report = service.GetProblemAccounts(viewModel);

            ExportExcel(viewModel, report.ProblemAccounts);
        }

        private void ExportExcel(GetListViewModel searchFilter, IEnumerable<ProblemAccountEntity> problemAccounts)
        {
            var roleName = ((int)searchFilter.SearchRoleId).DisplayRoleName();
            var fileName = string.Format(
                "{0} {1} - {2} - {3}.xls",
                ViewLang.ProblemAccount,
                searchFilter.FromDate.ToDateWithHyphen(),
                searchFilter.ToDate.ToDateWithHyphen(),
                roleName);
            var excelTemplatePath = Server.MapPath($"~/ExcelTemplates/ProblemAccountList/{roleName}.xls");

            using (var excelStream = excel.ExportExcel(excelTemplatePath, searchFilter, problemAccounts))
            {
                ExportToExcel(excelStream, fileName);
            }
        }

        [AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.ProblemAccountList, Policy = AuthenticationType.Host)]
        public ActionResult SetToReviewed(SetToReviewedViewModel viewModel)
        {
            viewModel.UserId = CurrentSession.UserId;
            viewModel.IP = CurrentContext.Ip4Address;

            return Json(service.SetToReviewed(viewModel), JsonRequestBehavior.DenyGet);
        }
    }
}
