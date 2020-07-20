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
            ViewBag.UserName = currentSession.UserName;
            ViewBag.ReturnUrl = ReturnUrl;

            return View();
        }

        public string Verify(string code)
        {
            string userName = currentSession.UserName;
            string userSecurityCode = SecurityCodeManager.GetSecurityCode(userName);

            if (userSecurityCode.Equals(code, StringComparison.CurrentCulture))
            {
                currentSession.UpdateLastActiveTime();

                return "valid";
            }

            return "invalid";
        }
    }
}