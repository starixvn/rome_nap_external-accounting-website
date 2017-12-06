namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
using System.Diagnostics;
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
    using ONEbook.UM.Filters;
    using ONEbook.UM.Models;
    using ONEbook.UM.Services;
    using ONEbook.UM.Services.Impl;

    public class AccountController : CaptchaMvc.Controllers.DefaultCaptchaController
    {
        private readonly ILoginService _userService;
        private readonly Core.Context.CurrentSession _currentSession;
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

        [HttpGet, CheckUMFilter]
        public ActionResult Login()
        {
            if (User.Identity.IsAuthenticated 
                && _currentSession.IsLogin)
            {
                return RedirectToHomePage();
            }

            SetUMTokenForInternalAlphaLogin();

            Encryptor.Instance.RandomCaptcha = new CaptchaResult().GetCaptchaString(4);

            return View(new LoginModel
            {
                Code = Encryptor.Instance.RandomCaptcha,
                LoginAttempt = _currentSession.LoginAttempt
            });
        }

        [HttpPost, CheckUMFilter]
        public ActionResult Login(LoginModel model)
        {
            if (!ValidateAntiXSRF(model.__RequestVerificationToken))
            {
                return View(new LoginModel(ViewLang.ExpiredLoginSession));
            }

            var rawPassword = DecryptPassword(model.Password, model.Code);
            if (string.IsNullOrEmpty(rawPassword))
            {
                return View(new LoginModel(ViewLang.LoginNetworkError));
            }

            if (!IsDisabledCaptcha(model)
                && IsRequiredCaptcha(model.UserName)
                && !IsValidCaptcha())
            {
                return CreateWrongValidationCaptchaView(model);
            }

            return VerifyLoginCredential(model, rawPassword);
        }

        private ActionResult CreateWrongValidationCaptchaView(LoginModel model)
            => View(
                new LoginModel
                {
                    UserName = model.UserName,
                    LoginAttempt = _currentSession.LoginAttempt,
                    LoginFailedMessage = ViewLang.WrongValidationCode
                });

        private bool IsRequiredCaptcha(string username)
            => _currentSession.LoginAttempt >= LoginModel.ThresholdFailToShowingCaptcha
                && _currentSession.UserAttempt.Equals(username, StringComparison.OrdinalIgnoreCase);

        private ActionResult VerifyLoginCredential(LoginModel model, string rawPassword)
        {
            int custId = 0, onuserId = 0;
            bool isIntAcc = true;

            var result = _userService.Login(
                model.UserName,
                rawPassword,
                new Request()
                {
                    Browser = Request.Browser.Browser,
                    CountryCode = _currentContext.CountryCode,
                    DomainName = _currentContext.DomainName,
                    IP = _currentContext.Ip4Address,
                    NetSessionId = _currentSession.NetSessionId,
                    Platform = Request.Browser.Platform
                },
                out custId,
                out onuserId,
                out isIntAcc);

            if (result != null
                && result.IsSuccess)
            {
                return LoginSuccess(model, custId, onuserId, isIntAcc, result);
            }

            UpdateLoginFailedAttempt(model);

            return View(new LoginModel
                        {
                            UserName = model.UserName,
                            LoginAttempt = _currentSession.LoginAttempt,
                            LoginFailedMessage = 
                                result != null 
                                ? result.ErrorMessage 
                                : ViewLang.LoginError
                        });
        }

        private void UpdateLoginFailedAttempt(LoginModel model)
        {
            if (_currentSession.UserAttempt.Equals(model.UserName, StringComparison.OrdinalIgnoreCase))
            {
                _currentSession.LoginAttempt++;
            }
            else
            {
                _currentSession.LoginAttempt = 1;
                _currentSession.UserAttempt = model.UserName;
            }
        }

        private bool IsDisabledCaptcha(LoginModel model)
        {
            var isDisableCaptcha =  
                _nocaptcha.AllowIPs
                    .Split(',')
                    .Contains(_currentContext.Ip4Address)
                && _nocaptcha.AllowAdmins
                    .Split(',')
                    .Any(admin => admin.Equals(model.UserName, StringComparison.OrdinalIgnoreCase));

            if(isDisableCaptcha)
            {
                ResetLoginAttempt();
            }

            return isDisableCaptcha;
        }

        private string DecryptPassword(string encryptedPassword, string key)
            => Encryptor.Instance.Decrypt(encryptedPassword, key);

        private ActionResult LoginSuccess(
            LoginModel model,
            int custId,
            int onuserId,
            bool isIntAcc,
            Result result)
        {
            ResetLoginAttempt();

            FormsAuthentication.SetAuthCookie(model.UserName, false);

            SetUserSession(model, custId, onuserId, isIntAcc, result.ResultData as Group);

            return isIntAcc || _currentSession.IsITAdmin
                ? RedirectToChooseGroupPage()
                : RedirectToHomePage();
        }

        private RedirectResult RedirectToChooseGroupPage()
            => Redirect("~/Account/ExternalGroup");

        private void SetUMTokenForInternalAlphaLogin()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }
        }

        private RedirectResult RedirectToHomePage()
            => Redirect("~/Home/Index/");

        private bool IsValidCaptcha()
        {
            try
            {
                return this.IsCaptchaValid(null);
            }
            catch (Exception ex)
            {
                ex.Write(Formater.LogType.Warning);

                return false;
            }
        }

        public ActionResult CaptchaImage(string prefix, bool noisy = true)
            => Content(new CaptchaResult().GetValiationImage(ControllerContext));

        #region Logout

        public ActionResult LogOff()
        {
            int onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                SignOut(onuserId, _currentSession.UserName);
            }

            Session.Clear();
            Session.RemoveAll();
            Session.Abandon();
            FormsAuthentication.SignOut();

            return RedirectToAction("Login");
        }

        public int ReCheckIn() => (int)GetRecheckinStatus();

        private RecheckinStatus GetRecheckinStatus()
        {
            if (_maintenanceService.IsUnderMaintenance())
            {
                return RecheckinStatus.Maintenance;
            }

            if (!Config.IsEnabledRecheckin)
            {
                return RecheckinStatus.Continue;
            }

            int onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                try
                {
                    var isKickOut = false;
                    _userService.Recheckin(onuserId, out isKickOut);
                    if (!isKickOut)
                    {
                        return RecheckinStatus.Continue;
                    }
                }
                catch (Exception ex)
                {
                ExceptionHandler.Handle(ex);
                    return RecheckinStatus.Error;
                }
            }

            return RecheckinStatus.KickedOut;
        }

        #endregion Logout

        #region UM Maintenance

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

            return RedirectToAction("Login");
        }

        public int ReCheckInOffline()
            => _maintenanceService.IsUnderMaintenance()
                ? (int)RecheckinStatus.Maintenance
                : (int)RecheckinStatus.Login;

        #endregion UM Maintenance

        [HttpGet, CheckUMFilter, Authorize]
        public ActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = _userService.GroupList();

            return View();
        }

        [CheckUMFilter, Authorize]
        public ActionResult ExternalGroupList()
        {
            var result = _userService.GroupList();

            return Json(
                result.Select(o => new
                {
                    GroupId = o.GroupID,
                    GroupName = o.GroupName
                }).ToArray(),
                JsonRequestBehavior.AllowGet);
        }

        public void ResetLoginAttempt()
        {
            _currentSession.LoginAttempt = 0;
        }

        private void SignOut(int onuserId, string userName)
            => _userService.Logout(onuserId, userName);

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
                    var exceptionLog = new Exception(ViewLang.CannotReadRequestValidationCookie);
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

        private void SetUserSession(LoginModel model, int custId, int onuserId, bool isIntAcc, Group group)
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
            _currentSession.IsLogin = true;
        }
    }
}