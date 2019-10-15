namespace Fanex.ExternalAccount.Core.Filters
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.Logging.Extensions.Web;
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Globalization;
    using System.Threading;
    using System.Web.Mvc;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class LogActionFilterAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsAppliedUserName())
            {
                var actionURL = GetActionLink(filterContext);
                if (!IsIgnoredAction(actionURL))
                {
                    var stopWatch = (Stopwatch)filterContext.HttpContext.Items["StopWatch"];
                    var duration = stopWatch == null ? -1 : stopWatch.ElapsedMilliseconds;
                    filterContext.HttpContext.Request.Headers["X-Request-Duration"] = duration.ToString();
                    Fanex.Logging.LogService.InfoAsync(CurrentSession.Instance.UserName + ": " + actionURL);
                    stopWatch?.Stop();
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsAppliedUserName() && !IsIgnoredAction(GetActionLink(filterContext)))
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                filterContext.HttpContext.Items["StopWatch"] = stopWatch;
            }
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private string GetActionLink(ControllerContext filterContext)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            var routingValues = filterContext.RouteData.Values;
            var currentArea = filterContext.RouteData.DataTokens["area"] ?? string.Empty;
            var currentController = (string)routingValues["controller"] ?? string.Empty;
            var currentAction = (string)routingValues["action"] ?? string.Empty;
            return string.Join("/", new[] { currentArea, currentController, currentAction });
        }

        private bool IsIgnoredAction(string actionURL)
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

        private bool IsAppliedUserName()
        {
            return !string.IsNullOrWhiteSpace(CurrentSession.Instance.UserName) && IsContainsIgnoreCase(AppSettings.LogActionFilter_AppliedUserNames, CurrentSession.Instance.UserName);
        }

#pragma warning disable S2325 // Methods and properties that don't access instance data should be static
        private bool IsContainsIgnoreCase(string source, string findingText)
#pragma warning restore S2325 // Methods and properties that don't access instance data should be static
        {
            return !string.IsNullOrWhiteSpace(findingText) && Thread.CurrentThread.CurrentCulture.CompareInfo.IndexOf(source, findingText, CompareOptions.IgnoreCase) >= 0;
        }
    }
}
