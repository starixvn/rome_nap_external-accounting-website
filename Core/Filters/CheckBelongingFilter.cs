namespace Fanex.ExternalAccount.Core.Filters
{
    using System;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Security;
    using Fanex.ExternalAccount.Core.Authorization;

    [AttributeUsage(AttributeTargets.All, Inherited = false, AllowMultiple = false)]
    public class CheckBelongingFilter : AuthorizeAttribute
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext.RequestContext.HttpContext.Request.Url.ToString().Contains("Shared/Offline"))
            {
                return;
            }

            base.OnAuthorization(filterContext);

            if (!authorization.IsBelong())
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
