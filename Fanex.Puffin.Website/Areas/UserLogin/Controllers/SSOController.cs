using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Fanex.Common;
using Fanex.NAP.Core.Configuration;
using Fanex.NAP.Core.Criteria.User;
using Fanex.NAP.Core.Model;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NAP.Core.Web.Auth;
using Fanex.NAP.Core.Web.Session;
using Fanex.Puffin.Admin.Criteria;
using Fanex.Puffin.Admin.Services.Implement;
using Fanex.Puffin.Admin.Services.Interface;
using Fanex.Puffin.Core.Services.Impl;
using Fanex.Puffin.Core.Web;
using Fanex.Puffin.Core.Web.Controllers;

namespace Fanex.Puffin.Website.Areas.UserLogin.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class SSOController : BaseController
    {
        #region members
        private readonly int _timeExipration = 30;
        private IUserService _userService;
        private IMenuItemService _menuItemService;
        private ICurrentSession _currentSession;
        private IAdminService _iAdminService;
        #endregion members

        #region ctors

        public SSOController()
            : this(null, null, null, null)
        {
        }

        public SSOController(
            IUserService userService,
            IMenuItemService menuItemService,
            ICurrentSession currentSession,
            IAdminService adminService)
        {
            _userService = userService ?? new UserService();
            _menuItemService = menuItemService ?? new MenuItemService();
            _currentSession = currentSession ?? new CurrentSession();
            _iAdminService = adminService ?? new AdminService();
        }

        #endregion ctors

        [AllowAnonymous]
        [HttpGet]
        public ActionResult Index(string userId, string onuserId, string time)
        {
            userId = Cryptography.CfsDeCrypt(userId.Replace(" ", "+"));
            onuserId = Cryptography.CfsDeCrypt(onuserId.Replace(" ", "+"));
            time = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
            string currentTime = _userService.UserSSOGetTime();

            var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(time);

            if (expiration.TotalSeconds <= _timeExipration)
            {
                #region do SSO check-in

                var criteria = new UserSSOCheckInUserCriteria
                {
                    UserId = Convert.ToInt32(userId),
                    SiteId = SiteId,
                    CheckOnUserId = Convert.ToInt32(onuserId),
                    OnUserId = 0,
                    IP = CurrentContext.IP,
                    NetSessionId = Session.SessionID,
                    LoginDate = DateTime.Now,
                    BrowserName = CurrentContext.BrowserName,
                    CountryCode = CurrentContext.CountryCode,
                    Domain = CurrentContext.DomainName,
                    Platform = CurrentContext.Platform
                };

                _userService.UserSSOCheckIn(ref criteria);

                #endregion do SSO check-in

                if (criteria.OnUserId != 0)
                {
                    var user = _userService.GetById(criteria.UserId) as User;

                    #region saves user session & auth info

                    _currentSession.UserId = user.Id;
                    _currentSession.Username = user.Username;
                    _currentSession.OnUserId = criteria.OnUserId;
                    _currentSession.SessionId = Session.SessionID.ToString();
                    _currentSession.Level = _userService.GetUserSiteLevel(user.Id, this.SiteId);

                    AppSession.CurrentUser = user;
                    AppSession.RoleAccess = PermissionService.GetRoleAccess((new FunctionService()).GetByUserAndSiteIncludeGroup(user.Id, NapSettings.Settings.SiteId, NapSettings.Settings.Environment));
                    //AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);

                    if (this.SiteId != 0)
                    {
                        _currentSession.MenuItemList = _menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                user.Id,
                                                                                SiteId,
                                                                                Environment,
                                                                                true) as IList<MenuItem>;
                        AppSession.CurrentUserMenu = _currentSession.MenuItemList;
                    }

                    CreateAuthInfo(user);

                    #endregion saves user session & auth info
                    return Redirect(FormsAuthentication.DefaultUrl);
                }
            }
            return Redirect(FormsAuthentication.LoginUrl);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult AccountingLogin(string username, string onuserId, string time)
        {
            Session.Clear();
            username = Cryptography.CfsDeCrypt(username.Replace(" ", "+"));
            onuserId = Cryptography.CfsDeCrypt(onuserId.Replace(" ", "+"));
            time = Cryptography.CfsDeCrypt(time.Replace(" ", "+"));
            string currentTime = _userService.UserSSOGetTime();

            var expiration = Convert.ToDateTime(currentTime) - Convert.ToDateTime(time);

            if (expiration.TotalSeconds <= _timeExipration)
            {
                #region do SSO check-in

                var criteria = new AccountingUserCheckOnlineStatusCriteria
                {
                    IsKickedOut = false,
                    On_UserID = Convert.ToInt32(onuserId)
                };

                var onlineStatus = _iAdminService.CheckUserOnlineStatus(criteria);

                #endregion do SSO check-in

                if (onlineStatus)
                {
                    var user = _userService.GetByUsername(username) as User;
                    if (user != null)
                    {
                        #region saves user session & auth info

                        _currentSession.UserId = user.Id;
                        _currentSession.Username = user.Username;
                        _currentSession.OnUserId = criteria.On_UserID;
                        _currentSession.SessionId = Session.SessionID.ToString();
                        _currentSession.Level = _userService.GetUserSiteLevel(user.Id, this.SiteId);

                        AppSession.CurrentUser = user;
                        AppSession.RoleAccess = PermissionService.GetRoleAccess((new FunctionService()).GetByUserAndSiteIncludeGroup(user.Id, NapSettings.Settings.SiteId, NapSettings.Settings.Environment));
                        //AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);

                        if (this.SiteId != 0)
                        {
                            _currentSession.MenuItemList = _menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                    user.Id,
                                                                                    SiteId,
                                                                                    Environment,
                                                                                    true) as IList<MenuItem>;
                            AppSession.CurrentUserMenu = _currentSession.MenuItemList;
                        }

                        CreateAuthInfo(user);

                        #endregion saves user session & auth info
                    }
                    
                    return Redirect(FormsAuthentication.DefaultUrl);
                }
            }
            return Redirect(FormsAuthentication.LoginUrl);
        }
        #region utilities

        private void CreateAuthInfo(User user)
        {
            var currentDomain = CurrentContext.GetCurrentDomain();

            var authInfo = new AuthInfo();

            authInfo.UserId = user.Id;
            authInfo.OnUserId = this._currentSession.OnUserId;

            var authTicket = new FormsAuthenticationTicket(
                 1,
                user.Username,
                DateTime.Now,
                DateTime.Now.AddMinutes(20000),
                false,
                authInfo.ToXml(),
                currentDomain.Path);

            string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
            var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket)
            {
                HttpOnly = true,
                Path = currentDomain.Path,
                Name = FormsAuthentication.FormsCookieName
            };

            Response.Cookies.Add(authCookie);
        }

        #endregion utilities
    }
}