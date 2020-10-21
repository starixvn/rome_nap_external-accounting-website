namespace Fanex.ExternalAccount.Controllers.SecurityCode
{
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Core.Context;
    using System;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SecurityCodeController : Controller
    {
        private readonly CurrentSession currentSession;

        public SecurityCodeController()
        {
            currentSession = new CurrentSession();
        }

        public ActionResult Index(string ReturnUrl)
        {
            ViewBag.MaxAllowAttempt = SecurityCodeManager.GetMaxAllowAttempt;
            ViewBag.UserName = currentSession.UserName;
            ViewBag.IsExternalSite = ReturnUrl.Contains("/updateCustomerSetting/updateSetting") 
                || ReturnUrl.Contains("/updateCustomerSetting/updateMultipleSetting");

            return View();
        }

        public ActionResult Verify(string txtSecurityCode)
        {
            string userSecurityCode = SecurityCodeManager.GetSecurityCode(currentSession.UserName);

            if (userSecurityCode.Equals(txtSecurityCode, StringComparison.CurrentCulture))
            {
                currentSession.UpdateLastActiveTime();
                currentSession.SCFailedCounter = 0;

                return Json(new
                {
                    StatusCode = 1
                });
            }

            return Json(new
            {
                StatusCode = 0,
                FailedCounter = ++currentSession.SCFailedCounter
            });
        }
    }
}