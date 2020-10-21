namespace Fanex.ExternalAccount.Core.Filters
{
    using Context;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Linq;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SecurityCodeFilterAttribute : BaseActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionURL = GetActionLink(filterContext);
            bool isFilteredUser = SecurityCodeManager.GetFilteredUsers().Contains(currentSession.UserName);
            bool isFilteredAction = IsContainsAction(actionURL, SecurityCodeManager.GetActions(isIgnored: false))
                && Utils.ToBoolean(filterContext.HttpContext.Request.Params.Get("isFilteredAction"), true);
            bool isIgnoredAction = IsContainsAction(actionURL, SecurityCodeManager.GetActions(isIgnored: true));

            if (isFilteredUser && isFilteredAction && IsSecurityCodeRequired())
            {
                filterContext.Result = RedirectToSecurityCodePage(actionURL);
            }

            if (isFilteredUser && !isIgnoredAction && !IsSecurityCodeRequired())
            {
                currentSession.UpdateLastActiveTime();
            }
        }

        private bool IsSecurityCodeRequired()
        {
            double scAliveTime = Utils.ToDouble(SecurityCodeManager.GetAliveTime) * 1000.0d;
            double currentTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);

            return currentTime - currentSession.LastActiveTime > scAliveTime;
        }

        private static RedirectResult RedirectToSecurityCodePage(string returnUrl) => new RedirectResult($"~/SecurityCode/Index?ReturnUrl={returnUrl}");

        private readonly CurrentSession currentSession = new CurrentSession();
    }
}