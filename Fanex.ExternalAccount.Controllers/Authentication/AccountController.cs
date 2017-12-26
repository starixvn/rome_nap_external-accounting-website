namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Linq;
    using System.Web.Helpers;
    using System.Web.Mvc;
    using System.Web.Security;
    using CaptchaMvc.Controllers;
    using CaptchaMvc.HtmlHelpers;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using DTO;
    using DTO.UserControl;
    using Module.Logs;
    using Module.Logs.Services;
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

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(4);

            return View(new LoginModel
            {
                Code = Encryptor.RandomCaptcha,
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
            int custId;
            int onuserId;
            bool isInternalAdmin;

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
                ? LoginSuccessful(model, custId, onuserId, isInternalAdmin, result.ResultData as Group)
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
            Group group)
        {
            ResetLoginAttempt();

            var canAccessPublicDomain = IsDenyAccessPublicDomain(isIntAcc, group);
            if (canAccessPublicDomain)
            {
                return View(new LoginModel(ViewLang.AccessDenyToPublicDomain));
            }

            FormsAuthentication.SetAuthCookie(model.UserName, false);

            SetUserSession(model, custId, onuserId, isIntAcc, group);

            return isIntAcc || _currentSession.IsITAdmin ? RedirectToChooseGroupPage() : RedirectToHomePage();
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
        [ExternalAccountAuthenticate]
        public ActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToAction("Index", "Home");
            }

            ViewBag.GroupList = _userService.GroupList();

            return View();
        }

        [ExternalAccountAuthenticate]
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

        private bool IsDenyAccessPublicDomain(bool isInternalAdmin, Group currentGroup)
        {
            if (GroupService.IsOfficeGroup(currentGroup.GroupName))
            {
                var isPublicDomain = Config.PublicDomains.Contains(_currentContext.DomainName);
                if (isPublicDomain)
                {
                    return GroupService.IsITGroup(currentGroup.GroupID)
                        ? Config.IsDenyITGroupAccessing
                        : isInternalAdmin
                            ? Config.IsDenyInternalGroupAccessing
                            : !GroupService.IsFinanceGroup(currentGroup.GroupName) || Config.IsDenyFinanceGroupAccessing;
                }
            }

            return false;
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
                    exceptionLog.Write(Formater.LogType.Warning);
                }
            }
            catch (Exception ex)
            {
                ex.Write(Formater.LogType.Warning);

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
            _currentSession.IsITAdmin = GroupService.IsITGroup(group.GroupID);
            _currentSession.FromSystem = 3;
            _currentSession.IsLogin = true;
        }
    }
}