namespace Fanex.ExternalAccount.Core.Filters
{
    using Context;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SecurityCodeFilterAttribute : BaseActionFilterAttribute
    {
        private readonly SecurityCodeManager _securityCodeManager;

        public SecurityCodeFilterAttribute(SecurityCodeManager securityCodeManager)
        {
            _securityCodeManager = securityCodeManager;
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            string actionURL = GetActionLink(filterContext);
            bool isFilteredUser = _securityCodeManager.GetFilteredUsers().Contains(currentSession.UserName);
            bool isFilteredAction = IsContainsAction(actionURL, _securityCodeManager.GetActions(isIgnored: false))
                && Utils.ToBoolean(filterContext.HttpContext.Request.Query["isFilteredAction"].ToString(), true);
            bool isIgnoredAction = IsContainsAction(actionURL, _securityCodeManager.GetActions(isIgnored: true));

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
            double scAliveTime = Utils.ToDouble(_securityCodeManager.GetAliveTime) * 1000.0d;
            double currentTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);

            return currentTime - currentSession.LastActiveTime > scAliveTime;
        }

        private static RedirectResult RedirectToSecurityCodePage(string returnUrl) => new RedirectResult($"/SecurityCode/Index?ReturnUrl={returnUrl}");

        private readonly CurrentSession currentSession = new CurrentSession();
    }
}
