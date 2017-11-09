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
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.UserControl;
    using Service.Impl.Login;
    using Service.Impl.Report;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Service.Interface.UserControl;
    using Utilities;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class ExternalAccountAuthenticate : AuthorizeAttribute, IAuthenticate
    {
        private readonly ILoginService _userService = new LoginService();
        private readonly ICachingService _cachingService = new CachingService();
        private readonly ICurrentSession _currentSession = new CurrentSession();
        private readonly ICurrentContext _currentContext = new CurrentContext();

        private bool _isAjax = true;
        private HttpRequest _request = null;
        private HttpResponse _response = null;
        private IList<Group> _groups = null;

        public ExternalAccountAuthenticate()
        {
            this._request = _currentContext.Context.Request;
            this._response = _currentContext.Context.Response;

            if (!_currentSession.IsLogin && !_currentContext.Context.Request.RawUrl.ToLower().Contains("login") && !_response.IsRequestBeingRedirected)
            {
                this._response.RedirectPermanent("~/Account/Login");
            }

            if (((_currentSession.IsLogin && _currentContext.Context.Request.RawUrl.ToLower().Contains("login")) ||
                (_currentSession.IsLogin && _currentContext.Context.Request.RawUrl.ToLower().Equals("/"))) && !_response.IsRequestBeingRedirected)
            {
                this._groups = _userService.GroupList().ToList();
                var group = _currentSession.IsInternalAdmin ? "/" + this._groups.ElementAt(0).GroupName : string.Empty;

                _response.RedirectPermanent("~/Home/Index" + group);
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

            this._groups = _userService.GroupList().ToList();
            var response = filterContext.HttpContext.Response;
            var request = filterContext.HttpContext.Request;

            if (this.IsHomeAuthorized())
            {
                return;
            }

            if (!_currentSession.IsLogin && !request.RawUrl.ToLower().Contains("login") && !response.IsRequestBeingRedirected)
            {
                response.RedirectPermanent("~/Account/Login");
            }

            if (((_currentSession.IsLogin && request.RawUrl.ToLower().Contains("login")) || (_currentSession.IsLogin && request.RawUrl.Equals("/")))
                 && !response.IsRequestBeingRedirected)
            {
                var group = _currentSession.IsInternalAdmin ? "/" + this._groups.ElementAt(0).GroupName : string.Empty;
                response.RedirectPermanent("~/Home/Index" + group);
            }

            try
            {
                var principal = filterContext.HttpContext.User;

                if (object.ReferenceEquals(principal, null) || object.ReferenceEquals(principal.Identity.AuthenticationType, null))
                {
                    this.CloseConnection(filterContext);
                    return;
                }

                if (!this.FunctionAuthorization(_currentSession.UserId, this.Function, this._request))
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

        public bool AllowReadByAuthenticationParam(string action)
        {
            return this.FunctionAuthorization(_currentSession.UserId, action, this._request, AuthenticateAction.AuthenticationType.Read);
        }

        public bool AllowUpdateByAuthenticationParam(string action)
        {
            return this.FunctionAuthorization(_currentSession.UserId, action, this._request, AuthenticateAction.AuthenticationType.Host);
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
            return this.FunctionAuthorization(_currentSession.UserId, this.Function, this._request);
        }

        public bool AllowUpdate(string action)
        {
            return this.FunctionAuthorization(_currentSession.UserId, this.Function, this._request);
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
            groupName = string.IsNullOrWhiteSpace(groupName) ? string.Empty : groupName;
            var userInfo = new UserService().GetUserById(_currentSession.UserId);

            if (_currentSession.GroupId != userInfo.GroupID)
            {
                _currentSession.IsInternalAdmin = _userService.CheckExternalGroup(_currentSession.UserId, 11, "ExternalGroup");
                _currentSession.GroupId = userInfo.GroupID;
                _currentSession.CurrencyGroup = userInfo.GroupName;
                _currentSession.IsITAdmin = _currentSession.GroupId == 1;
            }

            var allGroups = new GroupService().GetAllGroup();
            var groups = _userService.GroupList();
            var result = new ReportService().GetCustomerInfo(groupName, custId, username, getUplineInfo);
            var hasGroup = _currentSession.IsInternalAdmin ? allGroups.Any(o => o.GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase)) :
                groups.Where(o => o.GroupID == _currentSession.GroupId).First().GroupName.Equals(groupName, StringComparison.OrdinalIgnoreCase);

            return (result != null && result.Rows.Count > 0 && hasGroup) || ((_currentSession.IsInternalAdmin || custId == 0) && hasGroup);
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
            var onuserId = _currentSession.OnUserId;

            if (onuserId > 0)
            {
                this._userService.Logout(onuserId, _currentSession.UserName);
                _currentContext.Context.Session.Abandon();
            }
            else
            {
                _currentContext.Context.Session.Abandon();
            }

            if (!response.IsRequestBeingRedirected)
            {
                response.RedirectPermanent("~/Account/Login");
            }
        }

        private bool IsHomeAuthorized()
        {
            return this.Function.Equals("Home", StringComparison.OrdinalIgnoreCase) && _currentSession.IsLogin;
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
            Logs.Error(ex);

            this.SignOut(response);
        }        
    }
}