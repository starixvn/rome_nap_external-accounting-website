namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Text;
    using System.Web.Mvc;
    using System.Web.Security;
    using CustomerSetting.Security.Authenticate;
    using Utilities;

    [AttributeUsage(AttributeTargets.All, AllowMultiple = true)]
    public class ExternalAccountAuthenticateAttribute : AuthorizeAttribute, IAuthenticate
    {
        private readonly AuthorizationHandler authorization = new AuthorizationHandler();

        public bool IsAjaxRequest { get; set; } = true;

        public AuthenticateAction.AuthenticationType Policy { get; set; }

        public string Function { get; set; }

        public bool AllowRead(string action) 
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool AllowUpdate(string action) 
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true) 
            => authorization.IsBelong(groupName, custId, username, getUplineInfo);

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                return;
            }

            var currentSession = new CurrentSession();
            if (currentSession.HadNoSessionData)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            if(string.IsNullOrWhiteSpace(Function))
            {
                return;
            }

            if (!FunctionAuthorization(currentSession.UserId, Function, Policy)
                && !authorization.IsBelong())
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

        private void HandleUnauthorizedFunction(AuthorizationContext filterContext)
        {
            if (IsAjaxRequest)
            {
                filterContext.Result = new JsonResult
                {
                    Data = new
                    {
                        isSuccess = false,
                        errorCode = "1",
                        errorMessage = ViewLang.DontHavePermission
                    },
                    ContentEncoding = Encoding.UTF8,
                    ContentType = "application/json",
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                    MaxJsonLength = int.MaxValue,
                    RecursionLimit = int.MaxValue
                };
            }
            else
            {
                filterContext.Result = new PartialViewResult
                {
                    ViewName = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml"
                };
            }
        }
    }
}