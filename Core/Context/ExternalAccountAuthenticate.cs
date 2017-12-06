namespace Fanex.ExternalAccount.Core.Context
{
    using System.Text;
    using System.Web.Mvc;
    using Logging.Extensions.Web;
    using System.Web.Security;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Utilities;

    public class ExternalAccountAuthenticate : AuthorizeAttribute, IAuthenticate
    {
        private readonly ExternalAccountAuthorization authorization = new ExternalAccountAuthorization();

        public AuthenticateAction.AuthenticationType Policy { get; set; }

        public string Function { get; set; }

        public bool IsAjaxRequest { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            base.OnAuthorization(filterContext);

            if (filterContext.Result is HttpUnauthorizedResult)
            {
                // Already redirect to login page
                return;
            }

            var currentSession = new CurrentSession();
            if (currentSession.HadNoSessionData)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            if (!FunctionAuthorization(currentSession.UserId, Function, Policy))
            {
                HandleUnauthorizedFunction(filterContext);
            }
        }

        public bool AllowReadByAuthenticationParam(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, action, AuthenticateAction.AuthenticationType.Read);

        public bool AllowUpdateByAuthenticationParam(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, action, AuthenticateAction.AuthenticationType.Host);

        public bool FunctionAuthorization(int userId, string functionKey, AuthenticateAction.AuthenticationType authenticateActionType)
            => authorization.FunctionAuthorization(userId, functionKey, authenticateActionType, IsAjaxRequest);

        public bool AllowRead(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool AllowUpdate(string action)
            => FunctionAuthorization(CurrentSession.Instance.UserId, Function, Policy);

        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true)
            => authorization.IsBelong(groupName, custId, username, getUplineInfo);

        private void HandleUnauthorizedFunction(AuthorizationContext filterContext)
        {
            if (IsAjaxRequest)
            {
                filterContext.Result = new JsonResult()
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
                filterContext.Result = new PartialViewResult()
                {
                    ViewName = "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml"
                };
            }
        }
    }
}