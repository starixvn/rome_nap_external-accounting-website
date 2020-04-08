namespace Fanex.ExternalAccount.SearchAndCopy.Controllers
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;
    using Fanex.ExternalAccount.SearchAndCopy.Services;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class SearchAndCopyController : BaseController
    {
        private readonly IList<string> ErrorCodesNotSendEmail = new List<string> { "201", "202", "203" };
        private readonly ISearchAndCopyService searchAndCopyService;

        public SearchAndCopyController()
            : this(null)
        {
        }

        public SearchAndCopyController(ISearchAndCopyService searchAndCopyService)
        {
            this.searchAndCopyService = searchAndCopyService ?? new SearchAndCopyService(null, CurrentSession, CurrentContext);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult SearchCustomer(SearchCustomerRequest request)
        {
            var customer = searchAndCopyService.SearchCustomer(request.Username, Role.Super);
            return Json(customer);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        public ActionResult Super()
        {
            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult SuperPost(CloneSettingRequest request)
        {
            var result = searchAndCopyService.CloneSetting(request);
            //if (!ErrorCodesNotSendEmail.Contains(result.ErrorCode))
            //{
            //    var cloneSettingContext = new CloneSettingContext(request, result, CurrentSession.UserName);
            //    var emailBody = this.RenderRazorViewToString("Email", cloneSettingContext);
            //    searchAndCopyService.SendEmail(cloneSettingContext, emailBody);
            //}
            
            return Json(result);
        }
    }
}
