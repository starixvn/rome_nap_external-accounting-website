// --------------------------------------------------------------------------
// <copyright file="AuthorizationAttribute.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Authorize on URL & ajax request
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>

namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.Logging;

    /// <summary>
    /// Authorize action.
    /// </summary>
    public class AuthorizationAttribute : AuthorizeAttribute
    {
        private readonly ILoginService _userService = new LoginService();
        private readonly ICachingService _cachingService = new CachingService();
        private bool _isAjax = true;

        public AuthorizationAttribute()
        {
        }

        public string Function { get; set; }

        public EPolicy Policy { get; set; }

        public bool IsAjaxRequest 
        {
            get { return this._isAjax; } 
            set { this._isAjax = value; } 
        }

        /// <summary>
        /// Authorizes action.
        /// </summary>
        /// <param name="filterContext">AuthorizationContext object.</param>
        public override void OnAuthorization(AuthorizationContext filterContext)
        {
            if (this.IsHomeAuthorized())
            {
                return;
            }

            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

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
                if (filterContext.HttpContext == null)
                {
                    this.CloseConnection(filterContext);
                    return;
                }

                var principal = filterContext.HttpContext.User;

                if (object.ReferenceEquals(principal, null) || object.ReferenceEquals(principal.Identity.AuthenticationType, null))
                {
                    this.CloseConnection(filterContext);
                    return;
                }

                if (!this.FunctionAuthorization(currentSession.UserId, this.Function, request))
                {
                    this.CloseConnection(filterContext);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, response);
            }
        }

        /// <summary>
        /// Closes connection.
        /// </summary>
        /// <param name="filterContext">AuthorizationContext object.</param>
        private void CloseConnection(AuthorizationContext filterContext)
        {
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

            try
            {
                if (filterContext == null
                    || filterContext.HttpContext == null
                    || filterContext.HttpContext.Request == null)
                {
                    this.SignOut(response);
                    return;
                }

                if (response == null)
                {
                    this.SignOut(response);
                    return;
                }

                if (this.IsAjaxRequest)
                {
                    filterContext.Result = new JsonResult()
                    {
                        Data = new { IsSuccess = false, ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission },
                        ContentEncoding = Encoding.UTF8,
                        ContentType = "application/json",
                        JsonRequestBehavior = JsonRequestBehavior.AllowGet,
                        MaxJsonLength = int.MaxValue,
                        RecursionLimit = int.MaxValue
                    };
                }
                else if (!filterContext.HttpContext.Request.IsAuthenticated)
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
        /// <param name="response">Response object.</param>
        private void SignOut(HttpResponseBase response)
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

        private bool IsHomeAuthorized()
        {
            return this.Function.Equals("Home", StringComparison.OrdinalIgnoreCase) && CurrentSession.Instance.IsLogin;
        }

        /// <summary>
        /// Authorizes function.
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="request">Request object.</param>
        /// <returns>Boolean object.</returns>
        private bool FunctionAuthorization(int userId, string functionKey, HttpRequestBase request)
        {
            if (userId <= 0)
            {
                return false;
            }

            var userCache = this._cachingService.GetUserCache(userId);
            var menuList = userCache.MenuList;
            var foundItems = userCache.PolicyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = true;

            if (!request.RawUrl.Equals("/") && !request.RawUrl.ToLower().Contains("home/index") && 
                !request.RawUrl.ToLower().Contains("exporttoexcel") && !this.IsAjaxRequest)
            {
                hasPerm = menuList.Where(o => !string.IsNullOrWhiteSpace(o.Url) && 
                    request.RawUrl.ToLower().Contains(o.Url.ToLower().Replace("~", string.Empty))).Count() > 0;
            }

            if (!hasPerm)
            {
                return false;
            }

            if (foundItems.Count() > 0)
            {
                var firstItem = foundItems.First();
                switch (this.Policy)
                {
                    case EPolicy.Read:
                        if (firstItem.Policy == EPolicy.Create || firstItem.Policy == EPolicy.Delete || firstItem.Policy == EPolicy.Update || 
                            firstItem.Policy == EPolicy.Read)
                        {
                            return true;
                        }

                        break;
                    case EPolicy.Create:
                    case EPolicy.Delete:
                    case EPolicy.Update:
                        if (firstItem.Policy == EPolicy.Read)
                        {
                            return false;
                        }

                        return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Processes exception.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <param name="response">Response object.</param>
        /// <param name="category">Category string.</param>
        private void ProcessException(Exception ex, HttpResponseBase response, string category = "ExternalAccount")
        {
            LogService.Error(ex.Message, category, ex);

            this.SignOut(response);
        }
    }
}