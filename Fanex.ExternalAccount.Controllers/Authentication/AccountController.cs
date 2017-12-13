namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
using System.Diagnostics;
    using System.Linq;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Security;
    using CaptchaMvc.Controllers;
    using CaptchaMvc.HtmlHelpers;
    using Core.Context;
    using CustomerSetting.Module.Core.Web;
    using DTO;
    using DTO.UserControl;
    using Fanex.ExternalAccount.Service.Impl.Login;
    using Fanex.ExternalAccount.Service.Interface.Login;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Account;
using Fanex.Logging.Extensions.Web;
    using ONEbook.UM.Filters;
    using Service.Impl.Login;
    using Service.Interface.Login;
    using Utilities;
    using ViewModel.Account;
    using CurrentContext = Core.Context.CurrentContext;
    using CurrentSession = Core.Context.CurrentSession;

    public class AccountController : DefaultCaptchaController
    {
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;
        private readonly NoCaptchaSectionConfiguration _nocaptcha;
        private readonly ILoginService _userService;

        public AccountController()
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
            _userService = new LoginService();
            _nocaptcha = NoCaptchaSectionConfiguration.Instance;
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (_currentSession.IsAuthenticated)
            {
                return RedirectToHomePage();
            }

            SetUMTokenForInternalAlphaLogin();

            Encryptor.Instance.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);

            return View(new LoginModel
            {
                Code = Encryptor.Instance.RandomCaptcha,
                LoginAttempt = _currentSession.LoginAttempt
            });
        }

        [HttpPost]
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

        private ActionResult CreateWrongValidationCaptchaView(LoginModel model) => View(
            new LoginModel
            {
                UserName = model.UserName,
                LoginAttempt = _currentSession.LoginAttempt,
                LoginFailedMessage = ViewLang.WrongValidationCode
            });

        private bool IsRequiredCaptcha(string username) =>
            _currentSession.LoginAttempt >= LoginModel.ThresholdFailToShowingCaptcha
            && _currentSession.UserAttempt.Equals(username, StringComparison.OrdinalIgnoreCase);

        private ActionResult VerifyLoginCredential(LoginModel model, string rawPassword)
        {
            var custId = 0;
            var onuserId = 0;
            var isInternalAdmin = true;

            var result = _userService.Login(
                model.UserName,
                rawPassword,
                new Request
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
                out isInternalAdmin);

            return result?.IsSuccess == true
                ? LoginSuccessful(model, custId, onuserId, isInternalAdmin, result)
                : LoginFailed(model.UserName, result);
        }

        private ActionResult LoginFailed(string failedUsername, Result result)
        {
            UpdateLoginFailedAttempt(failedUsername);

            return View(new LoginModel
            {
                UserName = failedUsername,
                LoginAttempt = _currentSession.LoginAttempt,
                LoginFailedMessage =
                    result != null
                        ? result.ErrorMessage
                        : ViewLang.LoginError
            });
        }

        private void UpdateLoginFailedAttempt(string failedUsername)
        {
            if (_currentSession.UserAttempt.Equals(failedUsername, StringComparison.OrdinalIgnoreCase))
            {
                _currentSession.LoginAttempt++;
            }
            else
            {
                _currentSession.LoginAttempt = 1;
                _currentSession.UserAttempt = failedUsername;
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

            if (isDisableCaptcha)
            {
                ResetLoginAttempt();
            }

            return isDisableCaptcha;
        }

        private static string DecryptPassword(string encryptedPassword, string key) =>
            Encryptor.Decrypt(encryptedPassword, key);

        private ActionResult LoginSuccessful(
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

        private RedirectResult RedirectToChooseGroupPage() => Redirect("~/Account/ExternalGroup");

        private void SetUMTokenForInternalAlphaLogin()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

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

        public ActionResult CaptchaImage() => Content(CaptchaUtil.GetValiationImage(ControllerContext));

        public ActionResult LogOff()
        {
            var onuserId = _currentSession.OnUserId;
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

        [HttpGet]
        [CheckUMFilter]
        [Authorize]
        public ActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = _userService.GroupList();

            return View();
        }

        [CheckUMFilter]
        [Authorize]
        public ActionResult ExternalGroupList()
        {
            var result = _userService.GroupList();

            return Json(
                result.Select(o => new
                {
                    GroupId = o.GroupID,
                    o.GroupName
                }).ToArray(),
                JsonRequestBehavior.AllowGet);
        }

        private void ResetLoginAttempt()
        {
            _currentSession.LoginAttempt = 0;
        }

        private void SignOut(int onuserId, string userName)
        {
            _userService.Logout(onuserId, userName);
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
                if (!string.IsNullOrWhiteSpace(cookieValue?.Value))
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

        private void SetUserSession(LoginModel model, int custId, int onuserId, bool isInternalAdmin, Group group)
        {
            _currentSession.UserName = model.UserName;
            _currentSession.UserId = custId;
            _currentSession.CustId = custId;
            _currentSession.OnUserId = onuserId;
            _currentSession.IsInternalAdmin = isInternalAdmin;
            _currentSession.GroupId = group.GroupID;
            _currentSession.CurrencyGroup = group.GroupName;
            _currentSession.IsITAdmin = group.GroupID == 1;
            _currentSession.FromSystem = 3;
            _currentSession.IsLogin = true;
        }
    }
}