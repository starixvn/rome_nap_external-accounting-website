namespace Fanex.ExternalAccount.Core.Filters
{
    using Context;
    using Fanex.ExternalAccount.Core.Configurations.SecurityCode;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class SecurityCodeFilterAttribute : BaseActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var actionURL = GetActionLink(filterContext);
            if (IsContainsAction(actionURL, ignoredActions))
            {
                return;
            }

            var userName = currentSession.UserName;
            if (filteredUsers.Contains(userName) && IsContainsAction(actionURL, filteredActions))
            {
                if (IsSecurityCodeRequired())
                {
                    var returnUrl = HttpUtility.UrlEncode(filterContext.HttpContext.Request.RawUrl);
                    filterContext.Result = RedirectToSecurityCodePage(returnUrl);
                }
                else
                {
                    currentSession.UpdateLastActiveTime();
                }
            }

            if (filteredUsers.Contains(userName) && !IsContainsAction(actionURL, filteredActions) && !IsSecurityCodeRequired())
            {
                currentSession.UpdateLastActiveTime();
            }
        }

        private readonly string[] filteredActions = SecurityCodeManager.GetFilteredActions();

        private readonly string[] filteredUsers = SecurityCodeManager.GetFilteredUsers();

        private readonly string[] ignoredActions = SecurityCodeManager.GetIsIgnoredActions();

        private bool IsSecurityCodeRequired()
        {
            double scLastActiveTime = currentSession.LastActiveTime;
            double scLiveTime = Utils.ToDouble(SecurityCodeManager.GetLiveTime) * 1000.0d;
            double currentTime = Utils.ConvertDateTimeToMilliseconds(DateTime.Now);

            return currentTime - scLastActiveTime > scLiveTime;
        }

        private static RedirectResult RedirectToSecurityCodePage(string returnUrl) => new RedirectResult($"~/SecurityCode/Index?ReturnUrl={returnUrl}");

        private readonly CurrentSession currentSession = new CurrentSession();
    }
}