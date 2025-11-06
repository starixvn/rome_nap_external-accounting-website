namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Mvc.Filters;
    using System.Text.Json;
    using Authorization;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Utilities;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ExternalAccountAuthenticateAttribute : AuthorizeAttribute, IAuthorizationFilter, IAuthenticate
    {
        private readonly AuthorizationService authorization = new AuthorizationServiceImpl();

        public bool IsAjaxRequest { get; set; } = true;

        public AuthenticateAction.AuthenticationType Policy { get; set; }

        public string Function { get; set; }

        public bool AllowRead(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool AllowUpdate(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true)
            => authorization.IsBelong(ExtRequest.Instance.GetGroupName(groupName), custId, username);

        public void OnAuthorization(AuthorizationFilterContext filterContext)
        {
            // Check if user is authenticated first
            if (!filterContext.HttpContext.User.Identity?.IsAuthenticated ?? true)
            {
                filterContext.Result = new UnauthorizedResult();
                return;
            }

            var currentSession = new CurrentSession();
            if (currentSession.HadNoSessionData)
            {
                // Redirect to login page
                filterContext.Result = new RedirectResult("/Account/Login");
                return;
            }

            if (string.IsNullOrWhiteSpace(Function))
            {
                return;
            }

            if (!FunctionAuthorization(currentSession.UserId, Function, Policy))
            {
                HandleUnauthorizedFunction(filterContext);
            }
        }

        public bool AllowReadByAuthenticationParam(string action) => FunctionAuthorization(
            CurrentSession.Instance.UserId, action,
            AuthenticateAction.AuthenticationType.Read);

        public bool AllowUpdateByAuthenticationParam(string action) => FunctionAuthorization(
            CurrentSession.Instance.UserId, action,
            AuthenticateAction.AuthenticationType.Host);

        public bool FunctionAuthorization(int userId, string functionKey,
            AuthenticateAction.AuthenticationType authenticateActionType) =>
            authorization.IsAuthorized(userId, functionKey, authenticateActionType, IsAjaxRequest);

        private void HandleUnauthorizedFunction(AuthorizationFilterContext filterContext)
        {
            // Check if it's an AJAX request
            var isAjax = filterContext.HttpContext.Request.Headers["X-Requested-With"].ToString() == "XMLHttpRequest"
                || IsAjaxRequest;

            if (isAjax)
            {
                filterContext.Result = new JsonResult(new
                {
                    isSuccess = false,
                    errorCode = "1",
                    errorMessage = ViewLang.DontHavePermission
                });
                filterContext.HttpContext.Response.StatusCode = 403;
            }
            else
            {
                // For API controllers, return Forbidden instead of View
                filterContext.Result = new ForbidResult();
            }
        }
    }
}
