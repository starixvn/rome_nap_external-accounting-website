using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Utilities;
using System;
using System.Web.Mvc;

namespace Fanex.ExternalAccount.Controllers.SecurityCode
{
    public class SecurityCodeController : Controller
    {
        private readonly CurrentSession _currentSession;

        public SecurityCodeController()
        {
            _currentSession = new CurrentSession();
        }

        public ActionResult Index()
        {
            ViewBag.UserName = _currentSession.UserName;
            ViewBag.ErrorMessage = (string)TempData["ErrorMessage"] ?? string.Empty;

            return View();
        }

        public ActionResult Verify(string code)
        {
            string userName = _currentSession.UserName;
            string userSecurityCode = Config.GetSecurityCode(userName);

            if (userSecurityCode.Equals(code, StringComparison.CurrentCulture))
            {
                double idleTime = Utils.ToDouble(Config.GetSCIdleTime);
                double scExpiredTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now.AddMilliseconds(idleTime));

                HttpContext.Session.Add(CurrentSession.EASSCExpiredTime, scExpiredTime);

                return RedirectToReturnUrl("~/Home/Index");
            }

            TempData["ErrorMessage"] = ViewLang.SCInvalidMessage;

            return RedirectToAction("Index");
        }

        private RedirectResult RedirectToReturnUrl(string returnUrl) => Redirect(returnUrl);
    }
}