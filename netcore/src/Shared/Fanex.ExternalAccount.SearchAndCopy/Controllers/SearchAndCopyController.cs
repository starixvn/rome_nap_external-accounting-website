namespace Fanex.ExternalAccount.SearchAndCopy.Controllers
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Helpers;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.SearchAndCopy.Criteria;
    using Fanex.ExternalAccount.SearchAndCopy.Models;
    using Fanex.ExternalAccount.SearchAndCopy.Requests;
    using Fanex.ExternalAccount.SearchAndCopy.Services;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging.Extensions.Web;
    using Fanex.MailDelivery.Client;
    using Fanex.MailDelivery.Core.Models;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class SearchAndCopyController : BaseController
    {
        private readonly IList<string> ErrorCodesNotSendEmail = new List<string> { "201", "202", "203", "204", "205" };
        private readonly ISearchAndCopyService searchAndCopyService;
        private readonly CurrentSession _currentSession;

        public SearchAndCopyController()
            : this(null)
        {
            _currentSession = new CurrentSession();
        }

        public SearchAndCopyController(ISearchAndCopyService searchAndCopyService)
        {
            this.searchAndCopyService = searchAndCopyService ?? new SearchAndCopyService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        public JsonResult SearchCustomer(SearchCustomerRequest request)
        {
            var searchCustomerContext = new SearchCustomerContext(request, Role.Super, CurrentSession);
            var result = searchAndCopyService.ComposeSearchCustomerResult(searchAndCopyService.SearchCustomer(searchCustomerContext));
            return Json(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Read)]
        public IActionResult Super()
        {
            ViewBag.Code = Encryptor.RandomCaptcha;

            return PartialViewBase();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Host)]
        [ValidateAntiForgeryToken]
        public JsonResult SuperPost(CloneSettingRequest request)
        {
            var searchCustomerContext = new SearchCustomerContext(new SearchCustomerRequest { Username = request.ExistingUsername }, Role.Super, CurrentSession);
            var searchCustomerModel = searchAndCopyService.SearchCustomer(searchCustomerContext);
            var searchCustomerResult = searchAndCopyService.ComposeSearchCustomerResult(searchCustomerModel);
            if (!searchCustomerResult.IsSuccess)
            {
                return Json(searchCustomerResult);
            }

            var existingCustomer = (SearchCustomerModel)searchCustomerResult.ResultData;
            var cloneSettingContext = new CloneSettingContext(request, existingCustomer.CustId.Value, searchCustomerModel.Currency, searchCustomerModel.Site, CurrentSession, CurrentContext);
            var cloneSettingResult = searchAndCopyService.CloneSetting(cloneSettingContext);
            if (!cloneSettingResult.IsSuccess)
            {
                if (!ErrorCodesNotSendEmail.Contains(cloneSettingResult.ErrorCode))
                {
                    SendEmail(cloneSettingContext);
                }

                return Json(cloneSettingResult);
            }

            SendEmail(cloneSettingContext);
            return Json(cloneSettingContext.Result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SearchAndCopySuper, Policy = AuthenticationType.Read)]
        [ValidateAntiForgeryToken]
        [HttpPost]
        public IActionResult GetSuperPrefixes(SuperPrefixRequest query)
            => Json(searchAndCopyService.GetSuperPrefixes(new SuperPrefixCriteria
            {
                GroupName = _currentSession.ParentGroupName,
                Prefix = query.Prefix,
                AllowNewSuper = query.AllowNewSuper
            }));

        private void SendEmail(CloneSettingContext context)
        {
            try
            {
                var mailBody = this.RenderRazorViewToString("Email", context);
                var mail = new Mail
                {
                    Title = "Auto Generated Report Administrator",
                    Subject = $"New super created by external admin - {context.Request.Username}",
                    Body = mailBody,
                    MailingListId = Config.SearchAndCopySuperMailingListId
                };
                Task.Run(() =>
                {
                    try
                    {
                        MailClientHandler.Start();
                        MailClientHandler.SendFunctionMail(mail);
                        MailClientHandler.Stop();
                    }
                    catch (Exception ex)
                    {
                        ExceptionHandler.Handle(ex);
                    }
                });
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}