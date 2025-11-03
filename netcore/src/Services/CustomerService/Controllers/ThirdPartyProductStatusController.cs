namespace Fanex.ExternalAccount.Controllers.Customer
{
    using System.Web.Mvc;
    using Core.Base;
    using ViewModel.Customer;

    public class ThirdPartyProductStatusController : BaseController
    {
        public ActionResult Index(ThirdPartyProductStatusViewModel viewModel)
        {
            return View("~/Areas/CustomerSetting/Views/ThirdPartyProductStatus/Index.cshtml", viewModel);
        }
    }
}
