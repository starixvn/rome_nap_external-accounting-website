using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
using Fanex.ExternalAccount.Utilities;
using System;
using System.Web;
using System.Web.Mvc;
using SessionConstants = Fanex.ExternalAccount.Core.Context.CurrentSession;

namespace Fanex.ExternalAccount.Core.Filters
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SecurityCodeFilterAttribute : BaseActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionURL = GetActionLink(filterContext);
            if (!IsContainsAction(actionURL, filteredActions))
            {
                return;
            }

            var returnUrl = filterContext.HttpContext.Request.RawUrl;
            var session = filterContext.HttpContext.Session;
            if (IsSecurityCodeRequired(session))
            {
                filterContext.Result = RedirectToSecurityCodePage(returnUrl);
                return;
            }

            double scNewActiveTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);
            filterContext.HttpContext.Session[SessionConstants.EASSCLastActiveTime] = scNewActiveTime;
        }

        private readonly string[] filteredActions = SecurityCodeManager.GetFilteredActions();

        private static bool IsSecurityCodeRequired(HttpSessionStateBase session)
        {
            if (session[SessionConstants.EASSCLastActiveTime] == null)
            {
                return true;
            }

            double scLastActiveTime = (double)session[SessionConstants.EASSCLastActiveTime];
            double scLiveTime = Utils.ToDouble(SecurityCodeManager.GetLiveTime);
            double currentTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);

            return currentTime - scLastActiveTime > scLiveTime;
        }

        private static RedirectResult RedirectToSecurityCodePage(string returnUrl) => new RedirectResult($"~/SecurityCode/Index?ReturnUrl={returnUrl}");
    }
}