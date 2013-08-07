// -----------------------------------------------------------------------
// <copyright file="ExternalAccountAuthenticate.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Controllers.Context
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging;
    using Fanex.ExternalAccount.DTO;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalAccountAuthenticate : ActionFilterAttribute, IAuthenticate
    {
        private readonly ILoginService userService = new LoginService();
        private readonly ICachingService cachingService = new CachingService();

        private HttpRequest request = null;
        private HttpResponse response = null;

        public ExternalAccountAuthenticate()
        {
            var context = CurrentContext.Instance.Context;

            request = context.Request;
            response = context.Response;

            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            if (!currentSession.IsLogin && !request.RawUrl.ToLower().Contains("login"))
            {
                response.Redirect("~/Account/Login");
            }

            if ((currentSession.IsLogin && request.RawUrl.ToLower().Contains("login")) || (currentSession.IsLogin && request.RawUrl.Equals("/")))
            {
                var group = currentSession.IsInternalAdmin ? "/ACCDDD" : string.Empty;

                response.Redirect("~/Home/Index" + group);
            }
        }

        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = CurrentContext.Instance.Context;

            request = context.Request;
            response = context.Response;

            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            if (!currentSession.IsLogin && !request.RawUrl.ToLower().Contains("login"))
            {
                response.Redirect("~/Account/Login");
            }

            if ((currentSession.IsLogin && request.RawUrl.ToLower().Contains("login")) || (currentSession.IsLogin && request.RawUrl.Equals("/")))
            {
                var group = currentSession.IsInternalAdmin ? "/ACCDDD" : string.Empty;

                response.Redirect("~/Home/Index" + group);
            }

            try
            {
                if (context == null)
                {
                    this.CloseConnection(context);

                    return;
                }

                var principal = filterContext.HttpContext.User;

                if (object.ReferenceEquals(principal, null) || object.ReferenceEquals(principal.Identity.AuthenticationType, null))
                {
                    this.CloseConnection(context);
                    return;
                }

                if (!this.FunctionAuthorization(currentSession.UserId, this.Function, request))
                {
                    this.CloseConnection(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, response);
            }
        }

        public bool AllowRead(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, request);
        }

        public bool AllowUpdate(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, request);
        }

        public AuthenticateAction.AuthenticationType Policy
        {
            get;
            set;
        }

        public string Function
        {
            get;
            set;
        }

        private void CloseConnection(HttpContext httpContext)
        {
            var response = httpContext.Response;
            var request = httpContext.Request;

            try
            {
                if (httpContext == null
                    || httpContext.Request == null)
                {
                    this.SignOut(response);
                    return;
                }

                if (response == null)
                {
                    this.SignOut(response);
                    return;
                }

                if (request.Headers["X-Requested-With"] != null && request.Headers["X-Requested-With"].Equals("XMLHttpRequest"))
                {
                    httpContext.Response.ContentType = "application/json";
                    httpContext.Response.Write(new { IsSuccess = false, ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission });
                }
                else if (!httpContext.Request.IsAuthenticated)
                {
                    this.SignOut(response);
                }
                else
                {
                    response.Redirect("~/home/index", true);
                }
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, response);
            }
        }

        private void SignOut(HttpResponse response)
        {
            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            var onUserId = currentSession.OnUserId;

            if (onUserId > 0)
            {
                this.userService.Logout(onUserId, currentSession.UserName);
                currentContext.Context.Session.Abandon();
            }
            else
            {
                currentContext.Context.Session.Abandon();
            }

            response.Redirect("~/Account/Login", true);
        }

        private bool FunctionAuthorization(int userId, string functionKey, HttpRequest request)
        {
            if (userId <= 0)
            {
                return false;
            }

            var userCache = this.cachingService.GetUserCache(userId);
            var policyList = userCache.PolicyList;
            var menuList = userCache.MenuList;
            var foundItems = policyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = true;

            if (!request.RawUrl.Equals("/") && !request.RawUrl.ToLower().Contains("home/index") && !request.RawUrl.ToLower().Contains("exporttoexcel") && !(request.Headers["X-Requested-With"] != null && request.Headers["X-Requested-With"].Equals("XMLHttpRequest")))
            {
                hasPerm = menuList.Where(o => !string.IsNullOrWhiteSpace(o.Url) && request.RawUrl.ToLower().Contains(o.Url.ToLower().Replace("~", string.Empty))).Count() > 0;
            }

            if (!hasPerm && (foundItems == null || foundItems.Count() == 0))
            {
                return false;
            }

            if (foundItems.Count() > 0)
            {
                var firstItem = foundItems.First();
                switch (this.Policy)
                {
                    case AuthenticateAction.AuthenticationType.Read:
                        if (firstItem.Policy == ePolicy.Create ||
                            firstItem.Policy == ePolicy.Delete ||
                            firstItem.Policy == ePolicy.Update ||
                            firstItem.Policy == ePolicy.Read)
                        {
                            return true;
                        }

                        break;
                    case AuthenticateAction.AuthenticationType.Host:
                        if (firstItem.Policy == ePolicy.Read)
                        {
                            return false;
                        }

                        return true;
                }
            }

            return false;
        }

        private void ProcessException(Exception ex, HttpResponse response, string category = "ExternalAccount")
        {
            LogService.Error(ex.Message, category, ex);

            this.SignOut(response);
        }
    }
}