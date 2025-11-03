namespace Fanex.ProductStatus.Setting.Web
{
    using System;
    using System.Net;
    using System.Web.Mvc;
    using AppResources;
    using Configuration;
    using Models;

    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    internal class ProductStatusAuthorizeAttribute : AuthorizeAttribute
    {
        public bool IsHost { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                UnAuthorizedAction(filterContext);
                return;
            }

            var authorizationService = ProductStatusConfigurationHandler.AuthorizeService;

            var isAuthorized = IsHost
                ? authorizationService.CanUpdate(filterContext)
                : authorizationService.CanView(filterContext);

            if (!isAuthorized)
            {
                UnAuthorizedAction(filterContext);
            }
        }

        private static void UnAuthorizedAction(AuthorizationContext filterContext)
        {
            var response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                response.Clear();
                filterContext.Result = new JsonResult
                {
                    Data = new[] { new Result((int)HttpStatusCode.Forbidden, Resources.UnauthorizeMessage) },
                    JsonRequestBehavior = JsonRequestBehavior.DenyGet
                }; 
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult(ProductStatusConfigurationHandler.UnAuthorizedUrl);
            }
        }
    }
}
