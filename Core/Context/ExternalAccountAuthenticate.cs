// --------------------------------------------------------------------------
// <copyright file="ExternalAccountAuthenticate.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">StyleCop</change>
// </history>

namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalAccountAuthenticate : ActionFilterAttribute, IAuthenticate
    {
        private readonly ILoginService _userService = new LoginService();
        private readonly ICachingService _cachingService = new CachingService();

        private HttpRequest _request = null;
        private HttpResponse _response = null;

        public ExternalAccountAuthenticate()
        {
            var context = CurrentContext.Instance.Context;

            this._request = context.Request;
            this._response = context.Response;

            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            if (!currentSession.IsLogin && !this._request.RawUrl.ToLower().Contains("login"))
            {
                this._response.Redirect("~/Account/Login");
            }

            if ((currentSession.IsLogin && this._request.RawUrl.ToLower().Contains("login")) || (currentSession.IsLogin && this._request.RawUrl.Equals("/")))
            {
                var group = currentSession.IsInternalAdmin ? "/ACCDDD" : string.Empty;

                this._response.Redirect("~/Home/Index" + group);
            }
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

        /// <summary>
        /// Action executing context.
        /// </summary>
        /// <param name="filterContext">Filter context.</param>
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            var context = CurrentContext.Instance.Context;

            this._request = context.Request;
            this._response = context.Response;

            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            if (!currentSession.IsLogin && !this._request.RawUrl.ToLower().Contains("login"))
            {
                this._response.Redirect("~/Account/Login");
            }

            if ((currentSession.IsLogin && this._request.RawUrl.ToLower().Contains("login")) || (currentSession.IsLogin && this._request.RawUrl.Equals("/")))
            {
                var group = currentSession.IsInternalAdmin ? "/ACCDDD" : string.Empty;

                this._response.Redirect("~/Home/Index" + group);
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

                if (!this.FunctionAuthorization(currentSession.UserId, this.Function, this._request))
                {
                    this.CloseConnection(context);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, this._response);
            }
        }

        public bool AllowRead(string action)
        {
            if (action.Contains("SearchCustomer"))
            {
                return true;
            }

            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, this._request);
        }

        public bool AllowUpdate(string action)
        {
            if (action.Contains("SearchCustomer"))
            {
                return true;
            }

            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, this._request);
        }

        /// <summary>
        /// Close connection.
        /// </summary>
        /// <param name="httpContext">HttpContext object.</param>
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

        /// <summary>
        /// Sign out session.
        /// </summary>
        /// <param name="response">HttpResponse object.</param>
        private void SignOut(HttpResponse response)
        {
            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            var onuserId = currentSession.OnUserId;

            if (onuserId > 0)
            {
                this._userService.Logout(onuserId, currentSession.UserName);
                currentContext.Context.Session.Abandon();
            }
            else
            {
                currentContext.Context.Session.Abandon();
            }

            response.Redirect("~/Account/Login", true);
        }

        /// <summary>
        /// Authorizes function.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="request">HttpRequest object.</param>
        /// <returns>Boolean object.</returns>
        private bool FunctionAuthorization(int userId, string functionKey, HttpRequest request)
        {
            if (userId <= 0)
            {
                return false;
            }

            var userCache = this._cachingService.GetUserCache(userId);
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = true;
            if (!request.RawUrl.Equals("/") && !request.RawUrl.ToLower().Contains("home/index") && 
                !request.RawUrl.ToLower().Contains("exporttoexcel") && 
                !(request.Headers["X-Requested-With"] != null && 
                request.Headers["X-Requested-With"].Equals("XMLHttpRequest")))
            {
                hasPerm = userCache.MenuList.Where(o => !string.IsNullOrWhiteSpace(o.Url) && 
                    request.RawUrl.ToLower().Contains(o.Url.ToLower().Replace("~", string.Empty))).Count() > 0;
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
                        if (firstItem.Policy == EPolicy.Create ||
                            firstItem.Policy == EPolicy.Delete ||
                            firstItem.Policy == EPolicy.Update ||
                            firstItem.Policy == EPolicy.Read)
                        {
                            return true;
                        }

                        break;
                    case AuthenticateAction.AuthenticationType.Host:
                        if (firstItem.Policy == EPolicy.Read)
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