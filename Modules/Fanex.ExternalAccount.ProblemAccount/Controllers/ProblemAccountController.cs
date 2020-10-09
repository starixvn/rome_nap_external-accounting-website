namespace Fanex.ExternalAccount.ProblemAccount.Controllers
{
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
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
            => PartialViewBase(new IndexViewModel());

        [ExternalAccountAuthenticate(Function = eFunctions.ProblemAccountList, Policy = AuthenticationType.Read)]
        public ActionResult GetList(GetListViewModel viewModel)
            => Json(service.GetProblemAccounts(viewModel), JsonRequestBehavior.AllowGet);
    }
}
