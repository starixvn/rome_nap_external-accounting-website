// --------------------------------------------------------------------------
// <copyright file="ExternalAccountAuthenticate.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.13">StyleCop</change>
// </history>

namespace Fanex.ExternalAccount.Core.Context
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.ExternalAccount.Utilities;
    using Logging.Extensions.Web;
    using System.Web.Security;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalAccountAuthenticate : AuthorizeAttribute, IAuthenticate
    {
        private readonly ILoginService _userService = new LoginService();
        private readonly ICachingService _cachingService = new CachingService();
        private bool _isAjax = true;
        private HttpRequest _request = null;
        private HttpResponse _response = null;
        private IList<Group> _groups = null;

        public ExternalAccountAuthenticate()
        {
            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();

            this._request = currentContext.Context.Request;
            this._response = currentContext.Context.Response;

            if (!currentSession.IsLogin && !currentContext.Context.Request.RawUrl.ToLower().Contains("login") && !_response.IsRequestBeingRedirected)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            if (((currentSession.IsLogin && currentContext.Context.Request.RawUrl.ToLower().Contains("login")) ||
                (currentSession.IsLogin && currentContext.Context.Request.RawUrl.ToLower().Equals("/"))) && !_response.IsRequestBeingRedirected)
            {
                FormsAuthentication.RedirectFromLoginPage(currentSession.UserName, false);
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
            base.OnAuthorization(filterContext);

            this._groups = new LoginService().GroupList().ToList();
            var currentSession = new CurrentSession();
            var currentContext = new CurrentContext();
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

            if (this.IsHomeAuthorized())
            {
                return;
            }

            if (!currentSession.IsLogin && !request.RawUrl.ToLower().Contains("login") && !response.IsRequestBeingRedirected)
            {
                FormsAuthentication.RedirectToLoginPage();
            }

            if (((currentSession.IsLogin && request.RawUrl.ToLower().Contains("login")) || (currentSession.IsLogin && request.RawUrl.Equals("/")))
                 && !response.IsRequestBeingRedirected)
            {                
                FormsAuthentication.RedirectFromLoginPage(currentSession.UserName, false);
            }
        }

        public bool AllowReadByAuthenticationParam(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, action, this._request, AuthenticateAction.AuthenticationType.Read);
        }

        public bool AllowUpdateByAuthenticationParam(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, action, this._request, AuthenticateAction.AuthenticationType.Host);
        }

        /// <summary>
        /// Authorizes function .
        /// </summary>
        /// <param name="userId">Admin user id.</param>
        /// <param name="functionKey">Function key.</param>
        /// <param name="request">Request object.</param>
        /// <param name="authenticateActionType">Type of authenticate.</param>
        /// <returns>Boolean object.</returns>
        public bool FunctionAuthorization(int userId, string functionKey, HttpRequest request, AuthenticateAction.AuthenticationType authenticateActionType)
        {
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

            if (!hasPerm && (foundItems == null || foundItems.Count() == 0))
            {
                return false;
            }

            if (foundItems.Count() > 0)
            {
                var firstItem = foundItems.First();
                switch (authenticateActionType)
                {
                    case AuthenticateAction.AuthenticationType.Read:
                        if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update || firstItem.Policy == EPolicy.Delete ||
                            firstItem.Policy == EPolicy.Create)
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

        public bool AllowRead(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, this._request);
        }

        public bool AllowUpdate(string action)
        {
            return this.FunctionAuthorization(CurrentSession.Instance.UserId, this.Function, this._request);
        }

        /// <summary>
        /// Checks belonging.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="username">Customer username.</param>
        /// <param name="groupName">Group name.</param>
        /// <param name="getUplineInfo">Gets up-line or not.</param>
        /// <returns>Boolean object.</returns>
        public bool IsBelong(int custId, string username, string groupName = null, bool getUplineInfo = true)
        {
            var currentSession = CurrentSession.Instance;
            groupName = string.IsNullOrWhiteSpace(groupName) ? string.Empty : groupName;
            var loginService = new LoginService();
            var userInfo = new UserService().GetUserById(currentSession.UserId);

            if (currentSession.GroupId != userInfo.GroupID)
            {
                currentSession.IsInternalAdmin = loginService.CheckExternalGroup(currentSession.UserId, 11, "ExternalGroup");
                currentSession.GroupId = userInfo.GroupID;
                currentSession.CurrencyGroup = userInfo.GroupName;
                currentSession.IsITAdmin = currentSession.GroupId == 1;
            }

            var allGroups = new GroupService().GetAllGroup();
            var groups = loginService.GroupList();
            var result = new ReportService().GetCustomerInfo(groupName, custId, username, getUplineInfo);
            var hasGroup = currentSession.IsInternalAdmin ? allGroups.Any(o => o.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase)) :
                groups.Where(o => o.GroupID == currentSession.GroupId).First().GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);

            return (result != null && result.Rows.Count > 0 && hasGroup) || ((currentSession.IsInternalAdmin || custId == 0) && hasGroup);
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
                        Data = new { isSuccess = false, errorCode = "1", errorMessage = ViewLang.DontHavePermission },
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

            FormsAuthentication.SignOut();            
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
        private bool FunctionAuthorization(int userId, string functionKey, HttpRequest request)
        {
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
                        if (firstItem.Policy == EPolicy.Read || firstItem.Policy == EPolicy.Update || firstItem.Policy == EPolicy.Delete ||
                            firstItem.Policy == EPolicy.Create)
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

        /// <summary>
        /// Processes exception.
        /// </summary>
        /// <param name="ex">Exception object.</param>
        /// <param name="response">Response object.</param>
        private void ProcessException(Exception ex, HttpResponseBase response)
        {
            ExceptionHandler.Handle(ex);

            this.SignOut(response);
        }        
    }
}