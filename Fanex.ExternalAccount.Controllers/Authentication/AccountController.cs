// --------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="">
// Copyright (c) . All rights reserved.
// </copyright>
// <summary>
// Controller for account
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Created</change>
// </history>

using System;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Web.Helpers;
using System.Web.Mvc;
using System.Web.Security;
using CaptchaMvc.HtmlHelpers;
using Fanex.CustomerSetting.Module.Core.Web;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.DTO.UserControl;
using Fanex.ExternalAccount.Service.Impl.Login;
using Fanex.ExternalAccount.Service.Interface.Login;
using Fanex.ExternalAccount.Utilities;
using Fanex.ExternalAccount.ViewModel.Account;
using Fanex.Logging.Extensions.Web;
using ONEbook.UM.Models;
using ONEbook.UM.Services;
using ONEbook.UM.Services.Impl;
using UAParser;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    /// <summary>
    /// Account controller.
    /// </summary>
    public class AccountController : CaptchaMvc.Controllers.DefaultCaptchaController
    {
        private readonly ILoginService _userService;

        private readonly ICurrentSession _currentSession;

        private readonly ICurrentContext _currentContext;

        private readonly IMaintenanceService _maintenanceService;

        private readonly NoCaptchaSectionConfiguration _nocaptcha;

        public AccountController()
        {
            _currentContext = new Core.Context.CurrentContext();
            _currentSession = new Core.Context.CurrentSession();
            _userService = new LoginService();
            _maintenanceService = new MaintenanceService();
            _nocaptcha = NoCaptchaSectionConfiguration.Instance;
        }



        /// <summary>
        /// Gets login page.
        /// </summary>
        /// <returns>Login page.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            ViewBag.ShowCaptcha = ((Core.Context.CurrentSession)_currentSession).LoginAttempt > 2 ? string.Empty : "none";
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }

            if (_maintenanceService.IsUnderMaintenance())
            {
                return RedirectToAction("Offline", "Account");
            }

            if (_currentSession.IsLogin 
                && User.Identity.IsAuthenticated
                && _currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = _currentSession.IsInternalAdmin ? _userService.GroupList().First().GroupName : string.Empty;

                return Redirect("~/Home/Index/" + group);
            }

            var captcha = new CaptchaResult().GetCaptchaString(4);
            Encryptor.Instance.Code = captcha;

            return PartialView(new Login { Code = captcha });
        }

        /// <summary>
        /// Performs login action.
        /// </summary>
        /// <param name="model">Login model object.</param>
        /// <returns>JSON string.</returns>
        [SuppressMessage("StyleCop.CSharp.Nexcel.NexcelCustomRules", "SP2101:MethodMustNotContainMoreLinesThan", Justification = "Reviewed. Suppression is OK here.")]
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Login")]
        public ActionResult LoginPost(Login model)
        {
            if (!ValidateAntiXSRF(model.__RequestVerificationToken))
            {
                Core.Context.CurrentContext.LoginErrorMessage = "Your login session has expired. Please login again!";
                return RedirectToAction("Login", "Account");
            }

            var cipher = Encryptor.Instance;
            var password = cipher.Decrypt(model.Password, model.Code);
            if (password.Equals("cannot_decrypt_password", StringComparison.OrdinalIgnoreCase))
            {
                Core.Context.CurrentContext.LoginErrorMessage = "Your login page has not been loaded completely. Please check your network connection and try login again!";

                return RedirectToAction("Login", "Account");
            }

            var isValidCaptcha = false;
            try
            {
                isValidCaptcha = this.IsCaptchaValid(null);
            }
            catch
            {
                isValidCaptcha = false;
            }

            if ((_nocaptcha.AllowIPs.Split(',').Contains(_currentContext.Ip4Address) &&
                _nocaptcha.AllowAdmins.Split(',').Any(admin => admin.Equals(model.UserName, StringComparison.OrdinalIgnoreCase))) ||
                ((Core.Context.CurrentSession)_currentSession).LoginAttempt < 3 ||
                !((Core.Context.CurrentSession)_currentSession).UserAttempt.Equals(model.UserName, StringComparison.OrdinalIgnoreCase) ||
                isValidCaptcha)
            {
                FormsAuthentication.SetAuthCookie(model.UserName, false);
                Core.Context.CurrentContext.LoginErrorMessage = string.Empty;
                
                int custId = 0, onuserId = 0;
                bool isIntAcc = true;

                var result = _userService.Login(
                    model.UserName,
                    password,
                    GetRequestInfo(model.BrowserSize),
                    out custId,
                    out onuserId,
                    out isIntAcc);

                if (((Core.Context.CurrentSession)_currentSession).LoginAttempt > 2 &&
                    !((Core.Context.CurrentSession)_currentSession).UserAttempt.Equals(model.UserName, StringComparison.OrdinalIgnoreCase) &&
                    (result == null || !result.IsSuccess || !isValidCaptcha))
                {
                    ((Core.Context.CurrentSession)_currentSession).LoginAttempt = 1;
                    ((Core.Context.CurrentSession)_currentSession).UserAttempt = model.UserName;
                    ViewBag.ShowCaptcha = "none";

                    return RedirectToAction("Login", "Account");
                }

                if (result != null && result.IsSuccess)
                {
                    ((Core.Context.CurrentSession)_currentSession).LoginAttempt = 0;
                    var group = result.ResultData as Group;

                    RegisterLoginTicket(model, custId, onuserId, isIntAcc, group);

                    return isIntAcc || _currentSession.IsITAdmin ? RedirectToAction("ExternalGroup", "Account") : RedirectToAction("Index", "Home");
                }
                else
                {
                    if (string.IsNullOrWhiteSpace(((Core.Context.CurrentSession)_currentSession).UserAttempt) ||
                        ((Core.Context.CurrentSession)_currentSession).UserAttempt.Equals(model.UserName, StringComparison.OrdinalIgnoreCase))
                    {
                        ++((Core.Context.CurrentSession)_currentSession).LoginAttempt;
                    }
                    else
                    {
                        ((Core.Context.CurrentSession)_currentSession).LoginAttempt = 1;
                    }

                    ((Core.Context.CurrentSession)_currentSession).UserAttempt = model.UserName;
                    ViewBag.ShowCaptcha = ((Core.Context.CurrentSession)_currentSession).LoginAttempt > 2 ? string.Empty : "none";
                    Core.Context.CurrentContext.LoginErrorMessage = result.ErrorMessage;

                    return RedirectToAction("Login", "Account");
                }
            }
            else
            {
                ViewBag.ShowCaptcha = ((Core.Context.CurrentSession)_currentSession).LoginAttempt > 2 ? string.Empty : "none";
                Core.Context.CurrentContext.LoginErrorMessage = ViewLang.WrongValidationCode;
                _currentSession.UserName = string.Empty;

                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Gets captcha image.
        /// </summary>
        /// <param name="prefix">Prefix string.</param>
        /// <param name="noisy">True if specify noisy points.</param>
        /// <returns>ActionResult object.</returns>
        public ActionResult CaptchaImage(string prefix, bool noisy = true)
        {
            return Content(new CaptchaResult().GetValiationImage(ControllerContext));
        }

        #region Logout

        /// <summary>
        /// Logout session.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult LogOff()
        {
            int onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                return SignOut(onuserId, _currentSession.UserName);
            }
            else
            {
                _currentContext.Context.Session.Abandon();
            }

            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Recheck in.
        /// </summary>
        /// <returns>Integer object.</returns>
        public int ReCheckIn()
        {
            int onuserId = _currentSession.OnUserId;

            if (_maintenanceService.IsUnderMaintenance())
            {
                if (_currentSession.IsLogin)
                {
                    SignOut(onuserId, _currentSession.UserName);
                }

                return (int)StatusKickedOut.Maintenance;
            }

            var ignoreRecheckinCustomers = Config.IgnoreRecheckinCustomers
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

            var re_CheckIn = Config.Recheckin &&
                !ignoreRecheckinCustomers.Any(s => string.Equals(s, _currentSession.UserName, StringComparison.OrdinalIgnoreCase));

            var isKickOut = false;

            if (!re_CheckIn)
            {
                return (int)StatusKickedOut.Continue;
            }

            if (onuserId <= 0)
            {
                _currentContext.Context.Session.Abandon();
                return (int)StatusKickedOut.KickedOut;
            }

            try
            {
                _userService.Recheckin(onuserId, out isKickOut);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return (int)StatusKickedOut.Error;
            }

            if (!isKickOut)
            {
                return (int)StatusKickedOut.Continue;
            }
            else
            {
                SignOut(onuserId, _currentSession.UserName);
                return (int)StatusKickedOut.KickedOut;
            }
        }

        #endregion Logout

        #region UM Maintenance

        /// <summary>
        /// Under Maintenance.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult Offline()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }

            if (_maintenanceService.IsUnderMaintenance())
            {
                UnderMaintenanceTime underMaintenanceTime = _maintenanceService.GetUnderMaintenanceTime();
                ViewBag.UmFromTime = underMaintenanceTime.From;
                ViewBag.UmToTime = underMaintenanceTime.To;
                return PartialView();
            }
            else
            {
                return RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Process recheck-in offline mode.
        /// </summary>
        /// <returns>Integer number.</returns>
        public int ReCheckInOffline()
        {
            if (!_maintenanceService.IsUnderMaintenance())
            {
                return (int)StatusKickedOut.Login;
            }

            return (int)StatusKickedOut.Maintenance;
        }

        #endregion UM Maintenance

        /// <summary>
        /// Gets external groups.
        /// </summary>
        /// <returns>External groups page.</returns>
        [HttpGet]
        public ActionResult ExternalGroup()
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            if (_maintenanceService.IsUnderMaintenance())
            {
                return RedirectToAction("Offline", "Account");
            }

            if (!_currentSession.IsLogin && !_currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (_currentSession.IsLogin && _currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Index", "Home");
            }

            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = _userService.GroupList();

            return PartialView();
        }

        /// <summary>
        /// Gets external groups.
        /// </summary>
        /// <returns>External groups.</returns>
        public ActionResult ExternalGroupList()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                return RedirectToAction("Offline", "Account");
            }

            if (!_currentSession.IsLogin && !_currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Login", "Account");
            }

            if (_currentSession.IsLogin && _currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return RedirectToAction("Index", "Home");
            }

            var result = _userService.GroupList();

            return Json(result.Select(o => new { GroupId = o.GroupID, GroupName = o.GroupName }).ToArray(), JsonRequestBehavior.AllowGet);
        }

        public void ResetLoginAttempt()
        {
            ((Core.Context.CurrentSession)_currentSession).LoginAttempt = 0;
        }

        /// <summary>
        /// Processes sign out session.
        /// </summary>
        /// <param name="onuserId">On user id.</param>
        /// <param name="userName">Username of customer.</param>
        /// <returns>ActionResult object.</returns>
        private ActionResult SignOut(int onuserId, string userName)
        {
            try
            {
                _userService.Logout(onuserId, userName);
                _currentContext.Context.Session.Abandon();

                return RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
                return RedirectToAction("Error", "Shared");
            }
        }

        private bool ValidateAntiXSRF(string token)
        {
            try
            {
                var request = Request;

                var formToken = token;
                if (string.IsNullOrWhiteSpace(formToken))
                {
                    return false;
                }

                var cookieValue = request.Cookies[AntiForgeryConfig.CookieName];
                if (cookieValue != null
                    && !string.IsNullOrWhiteSpace(cookieValue.Value))
                {
                    AntiForgery.Validate(cookieValue.Value, formToken);
                }
                else
                {
                    var exceptionLog = new Exception("Cannot read Request Verification Token from cookie.");
                    ExceptionHandler.Handle(exceptionLog, TraceEventType.Warning);
                }
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);

                return false;
            }

            return true;
        }

        /// <summary>
        /// Registers a session login ticket.
        /// </summary>
        /// <param name="model">Login object.</param>
        /// <param name="custId">Identify of customer.</param>
        /// <param name="onuserId">Identify of login user.</param>
        /// <param name="isIntAcc">Is internal account or not.</param>
        /// <param name="group">Group object.</param>
        private void RegisterLoginTicket(Login model, int custId, int onuserId, bool isIntAcc, Group group)
        {
            _currentSession.UserName = model.UserName;
            _currentSession.UserId = custId;
            _currentSession.CustId = custId;
            _currentSession.OnUserId = onuserId;
            _currentSession.IsInternalAdmin = isIntAcc;
            _currentSession.GroupId = group.GroupID;
            _currentSession.CurrencyGroup = group.GroupName;
            _currentSession.IsITAdmin = group.GroupID == 1;
            _currentSession.FromSystem = 3;
        }

        private Request GetRequestInfo(string browserSize)
        {
            var userAgent = Request.UserAgent ?? string.Empty;
            var userAgentParser = Parser.GetDefault();
            var clientInfo = userAgentParser.Parse(userAgent);
            var clientUserAgent = clientInfo.UserAgent;

            var browser = $"{clientUserAgent.Family} {clientUserAgent.Major}.{clientUserAgent.Minor}.{clientUserAgent.Patch ?? "0"}, {browserSize}";
            var operateSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major ?? "0"}.{clientInfo.OS.Minor ?? "0"}.{clientInfo.OS.Patch ?? "0"}";
            var device = $"{clientInfo.Device.Family}, IsCrawler: {clientInfo.Device.IsSpider}";

            return new Request
            {
                Browser = browser,
                CountryCode = _currentContext.CountryCode,
                DomainName = _currentContext.DomainName,
                IP = _currentContext.Ip4Address,
                NetSessionId = _currentSession.NetSessionId,
                Platform = $"{operateSystem}, Device: {device}"
            };
        }
    }
}