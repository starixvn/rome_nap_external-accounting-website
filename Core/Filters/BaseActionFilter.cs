using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Shared;
using System;
using System.Globalization;
using System.Threading;
using System.Web.Mvc;

namespace Fanex.ExternalAccount.Core.Filters
{
    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class BaseActionFilterAttribute : ActionFilterAttribute
    {
        protected virtual string GetActionLink(ControllerContext filterContext)
        {
            var routingValues = filterContext.RouteData.Values;
            var currentArea = filterContext.RouteData.DataTokens["area"] ?? string.Empty;
            var currentController = (string)routingValues["controller"] ?? string.Empty;
            var currentAction = (string)routingValues["action"] ?? string.Empty;

            return string.Join("/", new[] { currentArea, currentController, currentAction });
        }

        protected virtual bool IsIgnoredAction(string actionURL)
        {
            string[] listIgnoreActions = AppSettings.LogActionFilter_IgnoredActions.Split(',');
            foreach (var ignoreAction in listIgnoreActions)
            {
                if (IsContainsIgnoreCase(actionURL, ignoreAction))
                {
                    return true;
                }
            }

            return false;
        }

        protected virtual bool IsAppliedUserName()
        {
            return !string.IsNullOrWhiteSpace(CurrentSession.Instance.UserName) && IsContainsIgnoreCase(AppSettings.LogActionFilter_AppliedUserNames, CurrentSession.Instance.UserName);
        }

        protected virtual bool IsContainsIgnoreCase(string source, string findingText)
        {
            return !string.IsNullOrWhiteSpace(findingText) && Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(source, findingText, CompareOptions.IgnoreCase) >= 0;
        }
    }
}