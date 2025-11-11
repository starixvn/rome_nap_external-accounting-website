namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Microsoft.AspNetCore.Mvc;
    using Core.Base;
    using ViewModel.Customer;

    public class ThirdPartyProductStatusController : BaseController
    {
        public IActionResult Index(ThirdPartyProductStatusViewModel viewModel)
        {
            return View("~/Areas/CustomerSetting/Views/ThirdPartyProductStatus/Index.cshtml", viewModel);
        }
    }
}
