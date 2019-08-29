namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using CaptchaMvc.Controllers;
    using CaptchaMvc.HtmlHelpers;
    using Core.Context;
    using Core.Helpers;
    using CustomerSetting.Module.Core.Web;
    using DTO;
    using DTO.UserControl;
    using Logging.Extensions.Web;
    using Service.Impl.Login;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Service.Interface.UserControl;
    using Utilities;
    using ViewModel.Account;
    using ViewModels;
    using CurrentContext = Core.Context.CurrentContext;
    using CurrentSession = Core.Context.CurrentSession;

    public class AccountController : DefaultCaptchaController
    {
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;
        private readonly NoCaptchaSectionConfiguration _nocaptcha;
        private readonly ILoginService _userService;
        private readonly IGroupService groupService;

        public AccountController()
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
            _userService = new LoginService();
            _nocaptcha = NoCaptchaSectionConfiguration.Instance;
            groupService = new GroupService();
        }

        [HttpGet]
        public ActionResult Login()
        {
            if (_currentSession.IsAuthenticated)
            {
                return RedirectToHomePage();
            }

            SetUMTokenForInternalAlphaLogin();

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);

            return View(new LoginModel
            {
                Code = Encryptor.RandomCaptcha,
                LoginAttempt = _currentSession.LoginAttempt,
                InvalidOTP = _currentSession.InvalidOTP
            });
        }

        public ActionResult LoginSso(string url, string line)
        {
            var pwd = _currentSession.HashPassword;
            var code = _currentSession.Captcha;
            var rawPassword = DecryptPassword(pwd, code);
            
            if (!_currentSession.IsAuthenticated ||
                string.IsNullOrWhiteSpace(pwd) ||
                string.IsNullOrWhiteSpace(code) ||
                string.IsNullOrEmpty(rawPassword))
            {
                return Redirect(url);
            }

            return Redirect(string.Format("{0}/Account/SwitchLine?username={1}&pwd={2}&code={3}&line={4}", url, _currentSession.UserName, pwd, code, line));
        }
        
        public ActionResult SwitchLine(string username, string pwd, string code, string line)
        {
            var rawPassword = DecryptPassword(pwd, code);
            if (Request.UrlReferrer == null || 
                string.IsNullOrWhiteSpace(Request.UrlReferrer.OriginalString) ||
                string.IsNullOrWhiteSpace(rawPassword))
            {
                return RedirectToAction("Login");
            }

            return VerifyLoginCredential(new LoginModel { UserName = username, Password = pwd, Code = code, Line = line }, rawPassword);
        }

        [HttpPost]
        public ActionResult Login(LoginModel model)
        {
            if (!AntiXSRFUtil.ValidateAntiXSRF(Request, model.__RequestVerificationToken))
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
                LoginFailedMessage = ViewLang.WrongValidationCode,
                InvalidOTP = _currentSession.InvalidOTP
            });

        private bool IsRequiredCaptcha(string username) =>
            (_currentSession.LoginAttempt >= LoginModel.ThresholdFailToShowingCaptcha
            && _currentSession.UserAttempt.Equals(username, StringComparison.OrdinalIgnoreCase))
            || _currentSession.InvalidOTP;

        private ActionResult VerifyLoginCredential(LoginModel model, string rawPassword)
        {
            int custId;
            int onuserId;
            bool hasMultipleGroupsPermission;

            var result = _userService.Login(
                model.UserName,
                rawPassword,
                    RequestHelper.GetRequestInfo(model.BrowserSize),
                out custId,
                out onuserId,
                out hasMultipleGroupsPermission);
            
            return result?.IsSuccess == true
                ? LoginSuccessful(model, custId, onuserId, hasMultipleGroupsPermission, result.ResultData as Group)
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
            bool hasMultipleGroupsPermission,
            Group group)
        {
            ResetLoginAttempt();

            var denyAccessPublicDomain = IsDenyAccessPublicDomain(hasMultipleGroupsPermission);
            if (denyAccessPublicDomain)
            {
                return View("~/Views/Account/Login.cshtml", new LoginModel(ViewLang.AccessDenyToPublicDomain));
            }

            FormsAuthentication.SetAuthCookie(model.UserName, false);

            SetUserSession(model, custId, onuserId, hasMultipleGroupsPermission, group);

            return RedirectOTPPage();
        }

        private void SetUMTokenForInternalAlphaLogin()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectResult RedirectOTPPage() => Redirect("~/OTP/Index/");

        private bool IsValidCaptcha()
        {
            try
            {
                return this.IsCaptchaValid(null);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);

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
        [ExternalAccountAuthenticate]
        public ActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToHomePage();
            }

            return View(new ExternalGroupViewModel
            {
                Username = _currentSession.UserName,
                GroupList = groupService.GetExternalGroup().ToList(),
                Code = _currentSession.OTPEncryptCode,
                EnabledOTP = _currentSession.EnabledOTP,
                ModifiedDate = _currentSession.OTPModifiedDate
            });
        }

        [ExternalAccountAuthenticate]
        public ActionResult ExternalGroupList()
        {
            var result = groupService.GetExternalGroup();

            return Json(
                result.Select(o => new
                {
                    GroupId = o.GroupID,
                    o.GroupName
                }).ToArray(),
                JsonRequestBehavior.AllowGet);
        }

        private bool IsDenyAccessPublicDomain(bool hasMultipleGroupsPermission)
        {
            string url = Request.Headers["Host"] + (Request.ApplicationPath.Length > 1 ? Request.ApplicationPath : string.Empty);
            var isPublicDomain = Config.PublicDomains.Any(url.Contains);
            return Config.IsRestrictAccessPublicDomain && isPublicDomain && hasMultipleGroupsPermission;
        }

        private void ResetLoginAttempt()
        {
            _currentSession.LoginAttempt = 0;
        }

        private void SignOut(int onuserId, string userName)
        {
            _userService.Logout(onuserId, userName);
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
            _currentSession.IsITAdmin = group.IsIT;
            _currentSession.FromSystem = 3;
            _currentSession.Captcha = model.Code;
            _currentSession.HashPassword = model.Password;
            _currentSession.CurrentLine = model.Line;
        }
    }
}