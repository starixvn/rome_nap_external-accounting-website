// --------------------------------------------------------------------------
// <copyright file="AccountController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// Controller for account
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.13">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Linq;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Account;

    /// <summary>
    /// Account controller.
    /// </summary>
    public class AccountController : Controller
    {
        private ILoginService _userService = null;

        private ICurrentSession _currentSession = null;

        private ICurrentContext _currentContext = null;

        private NoCaptchaSectionConfiguration _nocaptcha = null;

        public AccountController()
        {
            this._currentContext = new Fanex.ExternalAccount.Core.Context.CurrentContext();
            this._currentSession = new Fanex.ExternalAccount.Core.Context.CurrentSession();
            this._userService = new LoginService();
            this._nocaptcha = NoCaptchaSectionConfiguration.Instance;
        }

        /// <summary>
        /// Gets login page.
        /// </summary>
        /// <returns>Login page.</returns>
        [HttpGet]
        public ActionResult Login()
        {
            Response.AppendHeader("Content-Encoding", "gzip");
            Response.Filter = new System.IO.Compression.GZipStream(Response.Filter, System.IO.Compression.CompressionMode.Compress);

            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this._currentSession.IsLogin && !this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this._currentSession.IsLogin && this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                var group = this._currentSession.IsInternalAdmin ? this._userService.GroupList().First().GroupName : string.Empty;

                return this.Redirect("~/Home/Index/" + group);
            }

            return this.PartialView(new Login());
        }

        /// <summary>
        /// Performs login action.
        /// </summary>
        /// <param name="model">Login model object.</param>
        /// <returns>JSON string.</returns>
        [AllowAnonymous]
        [HttpPost]
        [ActionName("Login")]
        public ActionResult LoginPost(Login model)
        {
            if ((this._currentSession.Captcha != null && 
                this._currentSession.Captcha.Equals(model.Captcha, StringComparison.OrdinalIgnoreCase)) ||
                (this._nocaptcha.AllowIPs.Split(',').Contains(_currentContext.Ip4Address) &&
                this._nocaptcha.AllowAdmins.Split(',').Any(admin => admin.Equals(model.UserName, StringComparison.OrdinalIgnoreCase))))
            {
                Fanex.ExternalAccount.Core.Context.CurrentContext.LoginErrorMessage = string.Empty;
                int custId = 0, onuserId = 0;
                bool isIntAcc = true;
                var result = this._userService.Login(
                    model.UserName, 
                    model.Password, 
                    new DTO.Request() 
                    { 
                        Browser = Request.Browser.Browser, 
                        CountryCode = this._currentContext.CountryCode, 
                        DomainName = this._currentContext.DomainName, 
                        IP = this._currentContext.Ip4Address, 
                        NetSessionId = this._currentSession.NetSessionId,
                        Platform = Request.Browser.Platform
                    }, 
                    out custId, 
                    out onuserId, 
                    out isIntAcc);
                if (result != null && result.IsSuccess)
                {
                    var group = result.ResultData as Fanex.ExternalAccount.DTO.UserControl.Group;
                    this._currentSession.UserName = model.UserName;
                    this._currentSession.UserId = custId;
                    this._currentSession.CustId = custId;
                    this._currentSession.OnUserId = onuserId;
                    this._currentSession.IsInternalAdmin = isIntAcc;
                    this._currentSession.GroupId = group.GroupID;
                    this._currentSession.CurrencyGroup = group.GroupName;
                    this._currentSession.IsITAdmin = group.GroupID == 1;
                    Fanex.CustomerSetting.Module.Core.Web.AppSession.FromSystem = 3;
                    return isIntAcc || this._currentSession.IsITAdmin ? this.RedirectToAction("ExternalGroup", "Account") : this.RedirectToAction("Index", "Home");
                }
                else
                {
                    Fanex.ExternalAccount.Core.Context.CurrentContext.LoginErrorMessage = result.ErrorMessage;
                    return this.PartialView(model);
                }
            }
            else
            {
                Fanex.ExternalAccount.Core.Context.CurrentContext.LoginErrorMessage = ViewLang.WrongValidationCode;
                this._currentSession.UserName = string.Empty;
                return this.PartialView(model);
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
            return this.Content(new CaptchaResult().GetValiationImage(this.ControllerContext));
        }

        #region Logout

        /// <summary>
        /// Logout session.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult LogOff()
        {
            int onuserId = this._currentSession.OnUserId;
            if (onuserId > 0)
            {
                return this.SignOut(onuserId, this._currentSession.UserName);
            }
            else
            {
                this._currentContext.Context.Session.Abandon();
            }

            return this.RedirectToAction("Login", "Account");
        }

        /// <summary>
        /// Recheck in.
        /// </summary>
        /// <returns>Integer object.</returns>
        public int ReCheckIn()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();
            int onuserId = this._currentSession.OnUserId;

            if (maintenanceInfo.IsDownTime)
            {
                if (this._currentSession.IsLogin)
                {                    
                    this.SignOut(onuserId, this._currentSession.UserName);
                }

                return (int)StatusKickedOut.Maintenance;
            }

            var re_CheckIn = Config.Recheckin;
            var isKickOut = false;

            if (!re_CheckIn)
            {
                return (int)StatusKickedOut.Continue;
            }

            if (onuserId <= 0)
            {
                this._currentContext.Context.Session.Abandon();
                return (int)StatusKickedOut.KickedOut;
            }

            try
            {
                this._userService.Recheckin(onuserId, out isKickOut);
            }
            catch (Exception ex)
            {
                Logs.Error(ex.Message, "ExternalAccount", ex);
                return (int)StatusKickedOut.Error;
            }

            if (!isKickOut)
            {
                return (int)StatusKickedOut.Continue;
            }
            else
            {
                this.SignOut(onuserId, this._currentSession.UserName);
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
            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                ViewBag.UmFromTime = maintenanceInfo.TimeFrom;
                ViewBag.UmToTime = maintenanceInfo.TimeTo;
                return this.PartialView();
            }
            else
            {
                return this.RedirectToAction("Login", "Account");
            }
        }

        /// <summary>
        /// Process recheck-in offline mode.
        /// </summary>
        /// <returns>Integer number.</returns>
        public int ReCheckInOffline()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();
            if (!maintenanceInfo.IsDownTime)
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

            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this._currentSession.IsLogin && !this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this._currentSession.IsLogin && this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Index", "Home");
            }

            if (!this._currentSession.IsInternalAdmin)
            {
                return this.RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = this._userService.GroupList();

            return this.PartialView();
        }

        /// <summary>
        /// Gets external groups.
        /// </summary>
        /// <returns>External groups.</returns>
        public ActionResult ExternalGroupList()
        {
            var maintenanceInfo = Config.GetMaintenanceInfo();

            if (maintenanceInfo.IsDownTime)
            {
                return this.RedirectToAction("Offline", "Account");
            }

            if (!this._currentSession.IsLogin && !this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Login", "Account");
            }

            if (this._currentSession.IsLogin && this._currentContext.Context.Request.RawUrl.ToLower().Contains("login"))
            {
                return this.RedirectToAction("Index", "Home");
            }

            var result = this._userService.GroupList();

            return this.Json(result.Select(o => new { GroupId = o.GroupID, GroupName = o.GroupName }).ToArray(), JsonRequestBehavior.AllowGet);
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
                this._userService.Logout(onuserId, userName);
                this._currentContext.Context.Session.Abandon();

                return this.RedirectToAction("Login", "Account");
            }
            catch (Exception ex)
            {
                Logs.Error(ex.Message, "ExternalAccount", ex);
                return this.RedirectToAction("Error", "Shared");
            }
        }
    }
}