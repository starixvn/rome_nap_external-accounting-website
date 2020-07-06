using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
using Fanex.ExternalAccount.Utilities;
using System;
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
            if (!IsIgnoredAction(actionURL))
            {
                return;
            }

            var session = filterContext.HttpContext.Session;
            if (session == null)
            {
                filterContext.Result = new RedirectResult("~/Account/Login");
                return;
            }
            if (session[SessionConstants.EASSCExpiredTime] == null)
            {
                filterContext.Result = new RedirectResult("~/SecurityCode/Index?ReturnUrl=" + actionURL);
                return;
            }

            double scExpiredTime = (double)session[SessionConstants.EASSCExpiredTime];
            double currentTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);
            if (scExpiredTime < currentTime)
            {
                filterContext.Result = new RedirectResult("~/SecurityCode/Index?ReturnUrl=" + actionURL);
            }
        }

        protected override bool IsIgnoredAction(string actionURL)
        {
            string[] listFilteredActions = SecurityCodeManager.GetFilteredActions();
            foreach (var filteredActions in listFilteredActions)
            {
                if (IsContainsIgnoreCase(actionURL, filteredActions))
                {
                    return true;
                }
            }

            return false;
        }
    }
}