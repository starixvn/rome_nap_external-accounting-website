namespace Fanex.ExternalAccount.Core.Filters
{
    using Fanex.ExternalAccount.Core.Authorization;
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using ONEbook.UM;
    using ONEbook.UM.Services;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class CheckBelongingFilter : AuthorizeAttribute, IAuthorizationFilter
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        private readonly IMaintenanceService umService = new MaintenanceService(
                      UMClientManager.RestClient,
                      UMClientManager.CacheService,
                      UMClientManager.UMClientConfigs,
                      UMClientManager.HttpContext);

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // Check if user is authorized first
            if (!filterContext.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                return; // Let base authorization handle it
            }
            
            if (!umService.IsUnderMaintenance() && !authorization.IsBelong())
            {
                UnAuthorization(filterContext);                
            }
        }

        private static void UnAuthorization(AuthorizationFilterContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            // Check if it's an AJAX request
            var isAjaxRequest = filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest";

            if (isAjaxRequest)
            {
                // In ASP.NET Core, we don't need to clear response explicitly
                filterContext.Result = new ObjectResult(new { code = "401", responsetext = "You are not authorized on this function!" })
                {
                    StatusCode = 401
                };
            }
            else
            {
                filterContext.Result = new RedirectResult("/Account/LogOff");
            }

            filterContext.HttpContext.Session.Clear();
        }
    }
}
