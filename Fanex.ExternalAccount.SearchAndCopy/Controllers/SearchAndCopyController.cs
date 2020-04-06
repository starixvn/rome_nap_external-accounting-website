namespace Fanex.ExternalAccount.SearchAndCopy.Controllers
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;
    using Fanex.ExternalAccount.SearchAndCopy.Services;
    using System.Web.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class SearchAndCopyController : CustomerBase
    {
        private readonly ISearchAndCopyService searchAndCopyService;

        public SearchAndCopyController()
            : this(null)
        {
        }

        public SearchAndCopyController(ISearchAndCopyService searchAndCopyService)
        {
            this.searchAndCopyService = searchAndCopyService ?? new SearchAndCopyService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Read)]
        [AntiXSRFFilter]
        public JsonResult SearchCustomer(SearchCustomerRequest request)
        {
            var customer = searchAndCopyService.SearchCustomer(request.Username, Role.Super);
            return Json(customer);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Read)]
        public ActionResult Super()
        {
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult SuperPost(CloneSettingRequest request)
        {
            var result = searchAndCopyService.CloneSetting(request);
            return Json(result);
        }
    }
}
