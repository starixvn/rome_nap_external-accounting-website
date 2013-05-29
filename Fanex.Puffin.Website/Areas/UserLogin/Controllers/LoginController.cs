// --------------------------------------------------------------------------
// <copyright file="LoginController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Ricky.Nguyen" date="2012.12.12">Created</change>
// </history>

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Fanex.Common;
using Fanex.Logging;
using Fanex.NAP.Core.Configuration;
using Fanex.NAP.Core.Criteria.User;
using Fanex.NAP.Core.Model;
using Fanex.NAP.Core.Service.Impl;
using Fanex.NAP.Core.Service.Interface;
using Fanex.NAP.Core.Web;
using Fanex.NAP.Core.Web.Auth;
using Fanex.NAP.Core.Web.Session;
using Fanex.Puffin.Core.Configurations;
using Fanex.Puffin.Core.Resources;
using Fanex.Puffin.Core.Services.Impl;
using Fanex.Puffin.Core.Web;
using Fanex.Puffin.Core.Web.Controllers;

namespace Fanex.Puffin.Website.Areas.UserLogin.Controllers
{
    [SuppressMessage("Microsoft.StyleCop.CSharp.DocumentationRules", "SA1600:ElementsMustBeDocumented")]
    public class LoginController : BaseController
    {
        #region members

        private IUserService _userService;
        private IGroupService _groupService;
        private ISiteService _siteService;
        private IMenuItemService _menuItemService;
        private ICurrentSession _currentSession;
        private IFunctionService _functionService;
        private IMaintainanceService _maintenanceService;


        #endregion members

        #region ctors

        public LoginController()
            : this(null, null, null, null, null, null, null)
        {
        }

        public LoginController(
            IUserService userService,
            IGroupService groupService,
            ISiteService siteService,
            IMenuItemService menuItemService,
            ICurrentSession currentSession,
            IFunctionService functionService,
            IMaintainanceService maintenanceService)
        {
            _userService = userService ?? new UserService();
            _groupService = groupService ?? new GroupService();
            _siteService = siteService ?? new SiteService();
            _menuItemService = menuItemService ?? new MenuItemService();
            _currentSession = currentSession ?? new CurrentSession();
            _functionService = functionService ?? new FunctionService();
            _maintenanceService = maintenanceService ?? new MaintainanceService();

        }

        #endregion ctors

        #region helpers

        private void CreateUserMenu(User user)
        {
            _currentSession.MenuItemList = (IList<MenuItem>)_menuItemService.GetByUserAndSiteIncludeGroup(
                                                                                    user.Id,
                                                                                    SiteId,
                                                                                    Environment,
                                                                                    true);
            AppSession.CurrentUserMenu = _currentSession.MenuItemList;
        }

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

        #endregion helpers

        #region sign in

        [HttpGet]
        public ActionResult Index()
        {
            var maintenanceInfo = _maintenanceService.GetDowntimeInfo();

            if (maintenanceInfo.IsDowntime)
            {
                return RedirectToAction("Offline", "Common");
            }

            return View();
        }

        [HttpPost]
        public ActionResult Index(string username, string password)
        {
            if (!string.IsNullOrEmpty(username) && !string.IsNullOrEmpty(password))
            {
                var user = _userService.GetByUsername(username.Trim()) as User;

                try
                {
                    if (user != null)
                    {
                        password = Cryptography.CfsEnCode(password);

                        if (password == user.Password)
                        {
                            var group = user.Group == null ? null : _groupService.GetById(user.Group.Id);
                            var siteLevelList = _siteService.GetSiteLevelByUser(user.Id, Environment); // gets all sites of logging-in user

                            if ((group == null || group.IsActive) &&
                                siteLevelList.Where(x => x.IsActive).Any())
                            {
                                #region allows user login

                                #region check-in user

                                var criteria = new UserCheckInUserCriteria
                                {
                                    UserId = user.Id,
                                    SiteId = this.SiteId,
                                    NetSessionId = Session.SessionID,
                                    Ip = CurrentContext.IP,
                                    LoginDate = DateTime.Now,
                                    BrowserName = CurrentContext.BrowserName,
                                    CountryCode = CurrentContext.CountryCode,
                                    Domain = CurrentContext.DomainName,
                                    Platform = CurrentContext.Platform
                                };

                                _userService.UserCheckIn(ref criteria);

                                #endregion check-in user

                                #region saves user session

                                _currentSession.Level = _userService.GetUserSiteLevel(user.Id, this.SiteId);
                                _currentSession.UserId = user.Id;
                                _currentSession.Username = username;
                                _currentSession.OnUserId = criteria.OnUserId;
                                _currentSession.SessionId = Session.SessionID.ToString();

                                // current site is not NAP Hub
                                if (this.SiteId != 0)
                                {
                                    CreateUserMenu(user);
                                }

                                CreateAuthInfo(user);
                                AppSession.CurrentUser = user;
                                AppSession.RoleAccess = PermissionService.GetRoleAccess(_functionService.GetByUserAndSiteIncludeGroup(user.Id, NapSettings.Settings.SiteId, NapSettings.Settings.Environment));

                                #endregion saves user session

                                if (AppSession.CurrentUserMenu != null &&
                                    AppSession.CurrentUserMenu.Where(m => m.Parent == null).ToList().Count > 0)
                                {
                                    #region redirects user

                                    // if the user login directly, not through NAP Hub
                                    if (this.IsSiteLogin == 1)
                                    {
                                        #region checks if logging-in user is active on this site

                                        bool userIsActiveOnThisSite = false;

                                        foreach (var siteLevel in siteLevelList)
                                        {
                                            if (siteLevel.Site.Id == this.SiteId && siteLevel.IsActive)
                                            {
                                                userIsActiveOnThisSite = true;
                                            }
                                        }

                                        #endregion checks if logging-in user is active on this site

                                        if (userIsActiveOnThisSite)
                                        {
                                            //AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);
                                            return Redirect(FormsAuthentication.DefaultUrl);
                                        }
                                        else
                                        {
                                            DoSignOut();
                                            ViewBag.Message = ViewResources.Your_account_was_suspended;
                                        }
                                    }
                                    else
                                    {
                                        //AppSession.ViewIPVN = PermissionService.HasPermission(AccReportFunctionKeys.ViewIPVN);
                                        // user has access to multiple sites
                                        if (siteLevelList.Count() > 1)
                                        {
                                            return RedirectToAction("Site", "UserSite");
                                        }

                                        return RedirectToAction("GoToSite", "UserSite", new { id = siteLevelList.First().Site.Id });
                                    }

                                    #endregion redirects user
                                }
                                else
                                {
                                    // user has no menu permission
                                    DoSignOut();
                                    ViewBag.Message = Labels.You_have_no_menu_permission;
                                }

                                #endregion allows user login
                            }
                            else
                            {
                                // user account was suspended on all sites
                                if (siteLevelList.Count() > 0)
                                {
                                    ViewBag.Message = ViewResources.Your_account_was_suspended;
                                }
                                else
                                {
                                    // logging-in user belongs to no site
                                    ViewBag.Message = ViewResources.You_have_no_authorization;
                                }
                            }
                        }
                        else
                        {
                            // wrong password
                            ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                        }
                    }
                    else
                    {
                        // user doesn't exist
                        ViewBag.Message = ViewResources.Invalid_Username_or_Password;
                    }
                }
                catch (Exception ex)
                {
                    LogService.Error(ex.Message, ex);
                    throw ex;
                }
            }
            else
            {
                ViewBag.Message = "Invalid input!";
            }

            return View();
        }

        #endregion sign in

        #region sign out

        public ActionResult LogOut()
        {
            DoSignOut();

            return Redirect(FormsAuthentication.LoginUrl);
        }

        private void DoSignOut()
        {
            _userService.UserLogOut(new LogOutUserCriteria
            {
                OnUserId = _currentSession.OnUserId
            });
            FormsAuthentication.SignOut();
            _currentSession.ClearAll();
        }

        #endregion sign out

        #region re check in

        [HttpPost]
        public ActionResult ReCheckLogin()
        {
            try
            {
                if (!AppSettings.IsLocalTest)
                {
                    var maintenanceInfo = _maintenanceService.GetDowntimeInfo();

                    if (maintenanceInfo.IsDowntime)
                    {
                        return Json(new
                        {
                            IsKickedOut = "3"
                        });
                    }

                    var siteStatus = _siteService.GetStatusById(this.SiteId);

                    if (!siteStatus)
                    {
                        return Json(new
                        {
                            IsKickedOut = "2"
                        });
                    }

                    UserReCheckInUserCriteria criteria = new UserReCheckInUserCriteria();
                    criteria.OnUserId = Convert.ToInt32(_currentSession.OnUserId);
                    _userService.ReUserCheckIn(ref criteria);

                    return Json
                    (new
                    {
                        OnUserId = Convert.ToInt32(_currentSession.OnUserId),
                        IsKickedOut = criteria.IsKickedOut ? "1" : "0"
                    });

                    //return Json(criteria.IsKickedOut ? "1" : "0");
                }

                return Json(
                    (new
                    {
                        IsKickedOut = "0"
                    }));
            }
            catch (Exception ex)
            {
                LogService.Error(ex.Message, ex);
                throw ex;
            }
        }

        #endregion re check in
    }
}