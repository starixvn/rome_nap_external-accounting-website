namespace Fanex.ProductStatus.Setting.Web
{
    using System;
    using System.Net;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using AppResources;
    using Configuration;
    using Models;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class ProductStatusAuthorizeAttribute : AuthorizeAttribute, IAuthorizationFilter
    {
        public bool IsHost { get; set; }

        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var authorizationService = ProductStatusConfigurationHandler.AuthorizeService;

            var isAuthorized = IsHost
                ? authorizationService.CanUpdate(context)
                : authorizationService.CanView(context);

            if (!isAuthorized)
            {
                UnAuthorizedAction(context);
            }
        }

        private static void UnAuthorizedAction(AuthorizationFilterContext context)
        {
            var request = context.HttpContext.Request;
            var isAjaxRequest = request.Headers.ContainsKey("X-Requested-With") &&
                               request.Headers["X-Requested-With"].ToString().Equals("XMLHttpRequest", StringComparison.OrdinalIgnoreCase);

            if (isAjaxRequest)
            {
                context.Result = new JsonResult(new[] { new Result((int)HttpStatusCode.Forbidden, Resources.UnauthorizeMessage) });
            }
            else
            {
                context.Result = new RedirectResult(ProductStatusConfigurationHandler.UnAuthorizedUrl);
            }
        }
    }
}
