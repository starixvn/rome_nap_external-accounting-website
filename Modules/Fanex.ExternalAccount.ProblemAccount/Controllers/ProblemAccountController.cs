namespace Fanex.ExternalAccount.ProblemAccount.Controllers
{
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.ProblemAccount.Extensions;
    using Fanex.ExternalAccount.ProblemAccount.Resources;
    using Fanex.ExternalAccount.ProblemAccount.Services;
    using Fanex.ExternalAccount.ProblemAccount.ViewModels;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class ProblemAccountController : BaseController
    {
        private readonly IProblemAccountService service;

        public ProblemAccountController()
            : this(null)
        {
        }

        public ProblemAccountController(IProblemAccountService service)
        {
            this.service = new ProblemAccountService();
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
            var roleName = ((int)viewModel.SearchRoleId).DisplayRoleName();
            var fileName = string.Format(
                "{0} {1} - {2} - {3}.xls",
                ViewLang.ProblemAccount,
                viewModel.FromDate.ToDateWithHyphen(),
                viewModel.ToDate.ToDateWithHyphen(),
                roleName);
            var excelTemplatePath = Server.MapPath($"~/ExcelTemplates/ProblemAccountList/{roleName}.xls");

            using (var excelStream = service.ExportExcel(viewModel, excelTemplatePath))
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
