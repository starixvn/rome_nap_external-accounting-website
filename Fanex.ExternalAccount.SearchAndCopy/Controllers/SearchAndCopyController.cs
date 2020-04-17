namespace Fanex.ExternalAccount.SearchAndCopy.Controllers
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Helpers;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;
    using Fanex.ExternalAccount.SearchAndCopy.Services;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging.Extensions.Web;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
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
            this.searchAndCopyService = searchAndCopyService ?? new SearchAndCopyService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        [AntiXSRFFilter]
        public JsonResult SearchCustomer(SearchCustomerRequest request)
        {
            var searchCustomerContext = new SearchCustomerContext(request, Role.Super, CurrentSession);
            var result = searchAndCopyService.SearchCustomer(searchCustomerContext);
            return Json(result);
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
            var searchCustomerContext = new SearchCustomerContext(new SearchCustomerRequest { Username = request.ExistingUsername }, Role.Super, CurrentSession);
            var searchCustomerResult = searchAndCopyService.SearchCustomer(searchCustomerContext);
            if (!searchCustomerResult.IsSuccess)
            {
                return Json(searchCustomerResult);
            }

            var existingCustomer = (SearchCustomerModel)searchCustomerResult.ResultData;
            var cloneSettingContext = new CloneSettingContext(request, existingCustomer.CustId.Value, CurrentSession, CurrentContext);
            var cloneSettingResult = searchAndCopyService.CloneSetting(cloneSettingContext);
            if(!cloneSettingResult.IsSuccess)
            {
                if (!ErrorCodesNotSendEmail.Contains(cloneSettingResult.ErrorCode))
                {
                    Task.Run(() => SendEmail(cloneSettingContext));
                }

                return Json(cloneSettingResult);
            }

            Task.Run(() => SendEmail(cloneSettingContext));
            return Json(cloneSettingContext.Result);
        }

        private void SendEmail(CloneSettingContext context)
        {
            try
            {
                var mailBody = this.RenderRazorViewToString("Email", context);
                var mail = new Mail
                {
                    Title= "Auto Generated Report Administrator",
                    Subject = $"New super created by external admin - {context.Request.Username}",
                    Body = mailBody,
                    MailingListId = Config.SearchAndCopySuperMailingListId
                };
                MailClientHandler.Start();
                MailClientHandler.SendFunctionMail(mail);
                MailClientHandler.Stop();
            }
            catch(Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}
