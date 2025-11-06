namespace Fanex.ExternalAccount.Core.Filters
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class BaseActionFilterAttribute : ActionFilterAttribute
    {
        protected virtual string GetActionLink(ActionExecutingContext filterContext)
        {
            var routingValues = filterContext.RouteData.Values;
            var currentArea = routingValues.ContainsKey("area") ? routingValues["area"]?.ToString() ?? string.Empty : string.Empty;
            var currentController = routingValues.ContainsKey("controller") ? routingValues["controller"]?.ToString() ?? string.Empty : string.Empty;
            var currentAction = routingValues.ContainsKey("action") ? routingValues["action"]?.ToString() ?? string.Empty : string.Empty;

            return string.Join("/", new[] { currentArea, currentController, currentAction }.Where(s => !string.IsNullOrEmpty(s)));
        }

        protected virtual string GetActionLink(ActionExecutedContext filterContext)
        {
            var routingValues = filterContext.RouteData.Values;
            var currentArea = routingValues.ContainsKey("area") ? routingValues["area"]?.ToString() ?? string.Empty : string.Empty;
            var currentController = routingValues.ContainsKey("controller") ? routingValues["controller"]?.ToString() ?? string.Empty : string.Empty;
            var currentAction = routingValues.ContainsKey("action") ? routingValues["action"]?.ToString() ?? string.Empty : string.Empty;

            return string.Join("/", new[] { currentArea, currentController, currentAction }.Where(s => !string.IsNullOrEmpty(s)));
        }

        protected virtual bool IsContainsAction(string actionURL, string[] actions) => actions.Any(action => IsContainsCase(actionURL, action));

        protected virtual bool IsAppliedUserName()
        {
            return !string.IsNullOrWhiteSpace(CurrentSession.Instance.UserName) && IsContainsCase(AppSettings.LogActionFilter_AppliedUserNames, CurrentSession.Instance.UserName);
        }

        protected virtual bool IsContainsCase(string source, string findingText)
        {
            return !string.IsNullOrWhiteSpace(findingText) && Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(source, findingText, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
