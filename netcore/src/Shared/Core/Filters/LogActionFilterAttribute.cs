namespace Fanex.ExternalAccount.Core.Filters
{
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using System;
    using System.Diagnostics;
    using Microsoft.AspNetCore.Mvc.Filters;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class LogActionFilterAttribute : BaseActionFilterAttribute
    {
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (IsAppliedUserName())
            {
                var actionURL = GetActionLink(filterContext);
                if (!IsContainsAction(actionURL, ignoredActions))
                {
                    var items = filterContext.HttpContext.Items;
                    var stopWatch = items.ContainsKey("StopWatch") ? items["StopWatch"] as Stopwatch : null;
                    var duration = stopWatch == null ? -1 : stopWatch.ElapsedMilliseconds;
                    filterContext.HttpContext.Request.Headers["X-Request-Duration"] = duration.ToString();
                    Logging.LogService.InfoAsync(CurrentSession.Instance.UserName + ": " + actionURL);
                    stopWatch?.Stop();
                }
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (IsAppliedUserName() && !IsContainsAction(GetActionLink(filterContext), ignoredActions))
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                filterContext.HttpContext.Items["StopWatch"] = stopWatch;
            }
        }

        private readonly string[] ignoredActions = AppSettings.LogActionFilter_IgnoredActions.Split(',');
    }
}
