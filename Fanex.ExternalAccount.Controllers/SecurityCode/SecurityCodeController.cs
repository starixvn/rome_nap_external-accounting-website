using Fanex.CustomerSetting.Security.Authenticate;
using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;
using Fanex.ExternalAccount.Utilities;
using System;
using System.Web.Mvc;

namespace Fanex.ExternalAccount.Controllers.SecurityCode
{
    [ExternalAccountAuthenticate(Function = eFunctions.Administration,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SecurityCodeController : Controller
    {
        private readonly CurrentSession _currentSession;

        public SecurityCodeController()
        {
            _currentSession = new CurrentSession();
        }

        public ActionResult Index(string ReturnUrl)
        {
            ViewBag.UserName = _currentSession.UserName;
            ViewBag.ReturnUrl = ReturnUrl;

            return View();
        }

        public string Verify(string code)
        {
            string userName = _currentSession.UserName;
            string userSecurityCode = SecurityCodeManager.GetSecurityCode(userName);

            if (userSecurityCode.Equals(code, StringComparison.CurrentCulture))
            {
                double idleTime = Utils.ToDouble(SecurityCodeManager.GetIdleTime);
                double scExpiredTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now.AddMilliseconds(idleTime));

                HttpContext.Session.Add(CurrentSession.EASSCExpiredTime, scExpiredTime);

                return "valid";
            }

            return "invalid";
        }
    }
}