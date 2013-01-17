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

namespace Fanex.ExternalAccount.Controllers.Context
{
    using System;
    using System.Linq;
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Service.Interface.UserControl;
    using Fanex.Logging;

    public class AuthorizationAttribute : AuthorizeAttribute
    {
        private readonly ILoginService userService = new LoginService();
        private readonly ICachingService cachingService = new CachingService();
        private ICurrentSession currentSession = new CurrentSession();
        private ICurrentContext currentContext = new CurrentContext();

        public AuthorizationAttribute()
        {
        }

        public eFunctions Function { get; set; }

        public ePolicy Policy { get; set; }

        public override void OnAuthorization(AuthorizationContext filterContext)
        {
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

                if (!this.FunctionAuthorization(this.currentSession.UserId, (int)this.Function))
                {
                    this.CloseConnection(filterContext);
                    return;
                }
            }
            catch (Exception ex)
            {
                this.ProcessException(ex, filterContext.HttpContext.Response);
            }
        }

        private void CloseConnection(AuthorizationContext filterContext)
        {
            HttpResponseBase response = filterContext.HttpContext.Response;
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

                if (filterContext.HttpContext.Request.IsAjaxRequest())
                {
                    response.StatusCode = 200;
                    response.ContentType = "application/json";
                    response.Write("{\"IsSuccess\":false, \"ErrorMessage\":\"You don't have permission\"}");
                    response.Flush();
                    response.End();
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

        private void SignOut(HttpResponseBase response)
        {
            int onUserId = this.currentSession.OnUserId;
            if (onUserId > 0)
            {
                this.userService.Logout(onUserId, this.currentSession.UserName);
                this.currentContext.Context.Session.Abandon();
            }
            else
            {
                this.currentContext.Context.Session.Abandon();
            }

            response.Redirect("~/Account/Login", true);
        }

        private bool FunctionAuthorization(int userId, int functionId)
        {
            if (userId <= 0)
            {
                return false;
            }

            var userCache = this.cachingService.GetUserCache(userId);
            var policyList = userCache.PolicyList;
            var foundItems = policyList.Where(x => x.FuncID == functionId);
            if (foundItems.Count() > 0)
            {
                var firstItem = foundItems.First();
                switch (this.Policy)
                {
                    case ePolicy.Read:
                        if (firstItem.Policy == ePolicy.Create ||
                            firstItem.Policy == ePolicy.Delete ||
                            firstItem.Policy == ePolicy.Update ||
                            firstItem.Policy == ePolicy.Read)
                        {
                            return true;
                        }

                        break;
                    case ePolicy.Create:
                    case ePolicy.Delete:
                    case ePolicy.Update:
                        if (firstItem.Policy == ePolicy.Read)
                        {
                            return false;
                        }

                        return true;
                }
            }

            return false;
        }

        private void ProcessException(Exception ex, HttpResponseBase response, string category = "ExternalAccount")
        {
            LogService.Error(ex.Message, category, ex);

            this.SignOut(response);
        }
    }
}