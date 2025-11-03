namespace Fanex.ExternalAccount.Core.Filters
{
    using Fanex.ExternalAccount.Core.Authorization;
    using System;
    using System.Web;
    using System.Web.Mvc;
    using ONEbook.UM;
    using ONEbook.UM.Services;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class CheckBelongingFilter : AuthorizeAttribute
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        private readonly IMaintenanceService umService = new MaintenanceService(
                      UMClientManager.RestClient,
                      UMClientManager.CacheService,
                      UMClientManager.UMClientConfigs,
                      UMClientManager.HttpContext);

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);
            
            if (!umService.IsUnderMaintenance() && !authorization.IsBelong())
            {
                UnAuthorization(filterContext);                
            }
        }

        private static void UnAuthorization(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;

            if (filterContext.HttpContext.Request.IsAjaxRequest())
            {
                response.Clear();
                response.Write("{ \"code\": \"401\", \"responsetext\": \"You are not authorized on this function!\" }");
                filterContext.Result = new HttpUnauthorizedResult();
                filterContext.Result.ExecuteResult(filterContext.Controller.ControllerContext);
                response.TrySkipIisCustomErrors = true;
                response.End();
            }
            else
            {
                filterContext.Result = new RedirectResult("~/Account/LogOff");
            }

            var session = filterContext.HttpContext.Session;
            session.Clear();
            session.Abandon();            
        }
    }
}
