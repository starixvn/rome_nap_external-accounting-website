namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using ViewModel.Customer;

    public class ThirdPartyProductStatusController : BaseController
    {
        public ThirdPartyProductStatusController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
        }

        public IActionResult Index(ThirdPartyProductStatusViewModel viewModel)
        {
            return View("~/Areas/CustomerSetting/Views/ThirdPartyProductStatus/Index.cshtml", viewModel);
        }
    }
}
