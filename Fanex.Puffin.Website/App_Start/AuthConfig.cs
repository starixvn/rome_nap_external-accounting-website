// --------------------------------------------------------------------------
// <copyright file="AuthConfig.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2013.1.25">Created</change>
// </history>


using System.Web;
using System.Web.Security;
using Fanex.NAP.Core.Criteria.User;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Web.Auth;
using Fanex.NAP.Core.Web.Session;

namespace Fanex.Puffin.Website.App_Start
{
    public class AuthConfig
    {
        public static void RegisterAuth(HttpContext context)
        {
            var user = HttpContext.Current.User;

            if (user != null)
            {
                if (user.Identity is FormsIdentity && user.Identity.IsAuthenticated)
                {
                    string cookie = FormsAuthentication.FormsCookieName;
                    HttpCookie httpCookie = context.Request.Cookies[cookie];

                    if (httpCookie == null)
                    {
                        return;
                    }

                    var ticket = FormsAuthentication.Decrypt(httpCookie.Value);
                    if (ticket == null || ticket.Expired)
                    {
                        return;
                    }

                    var identity = new FormsIdentity(ticket);
                    if (string.IsNullOrEmpty(ticket.UserData))
                    {
                        return;
                    }

                    var udata = AuthInfo.CreateUserData(ticket.UserData);
                    var principal = new NapPrincipal(identity, udata);
                    context.User = principal;
                }
            }
        }

        public static void SignOut()
        {
            var _currentSession = new CurrentSession();
            var _userService = new UserService();
            var logOutUserCriteria = new LogOutUserCriteria();

            logOutUserCriteria.OnUserId = _currentSession.OnUserId;
            _userService.UserLogOut(logOutUserCriteria);
            _currentSession.ClearAll();
        }
    }
}