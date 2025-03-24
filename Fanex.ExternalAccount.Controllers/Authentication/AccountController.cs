namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using System.Web.Mvc;
    using System.Web.Security;
    using CaptchaMvc.Controllers;
    using CaptchaMvc.HtmlHelpers;
    using Core.Context;
    using Core.Helpers;
    using DTO;
    using DTO.UserControl;
    using Logging.Extensions.Web;
    using RSACryptoPad;
    using Service.Impl.Login;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Utilities;
    using ViewModel.Account;
    using ViewModels;
    using CurrentContext = Core.Context.CurrentContext;
    using CurrentSession = Core.Context.CurrentSession;

    public class AccountController : DefaultCaptchaController
    {
        private const int CaptchaLength = 4;
        private const string LoginActionName = "Login";
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;
        private readonly NoCaptchaSectionConfiguration _nocaptcha;
        private readonly ILoginService _userService;
        private readonly IGroupService groupService;

        private string _scLastActiveTime;
        private string _scFailedCounter;

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

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(CaptchaLength);

            return View(new LoginModel
            {
                Code = Encryptor.RandomCaptcha,
                LoginAttempt = _currentSession.LoginAttempt,
                InvalidOTP = _currentSession.InvalidOTP
            });
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

            var path = Server.MapPath("~/IbcKeys/ibcpkey.snk");
            var timeout = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff tt");
            var username = _currentSession.UserName;
            var fromLine = _currentSession.CurrentLine;
            var lastActiveTime = _currentSession.LastActiveTime;
            var failedCounter = _currentSession.SCFailedCounter;
            var parameter = $"{username}&{pwd}&{code}&{line}&{timeout}&{fromLine}&{lastActiveTime}&{failedCounter}";
#pragma warning disable S109 // Magic numbers should not be used
            parameter = new EncryptionThread().EncryptString(parameter, 1024, path);
#pragma warning restore S109 // Magic numbers should not be used

            var onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                SignOut(onuserId, _currentSession.UserName);
            }

            Session.Clear();

            WriteLogSwitchLine($"Switching {fromLine} to {line}, username: {username}", TraceEventType.Information);

            return Redirect($"{url}/Account/SwitchLine?param={parameter}");
        }

        public ActionResult SwitchLine(string param)
        {
            var path = Server.MapPath("~/IbcKeys/ibckey.snk");
            var parameter = new EncryptionThread().DecryptString(param.Replace(' ', '+'), 1024, path);
            var queries = parameter.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            var username = queries[0];
            var pwd = queries[1];
            var code = queries[2];
            var line = queries[3];
            var timeout = queries[4];
            var fromLine = queries[5];
#pragma warning disable S109 // Magic numbers should not be used
            _scLastActiveTime = queries[6];
            _scFailedCounter = queries[7];
#pragma warning restore S109 // Magic numbers should not be used
            var totalSeconds = DateTime.Now.Subtract(Convert.ToDateTime(timeout)).TotalSeconds;
            var isTimeout = totalSeconds > 10;
            var rawPassword = DecryptPassword(pwd, code);

            if (Request.UrlReferrer == null)
            {
                WriteLogSwitchLine($"Switched {fromLine} to {line} failed: call directly URL, username: {username}", TraceEventType.Warning);
                return RedirectToAction(LoginActionName);
            }

            if (isTimeout)
            {
                WriteLogSwitchLine($"Switched {fromLine} to {line} reached limit timeout: {totalSeconds}s, username: {username}", TraceEventType.Warning);
                return RedirectToAction(LoginActionName);
            }

            if (string.IsNullOrWhiteSpace(rawPassword))
            {
                WriteLogSwitchLine($"Switched line failed: wrong param info", TraceEventType.Warning);
                return RedirectToAction(LoginActionName);
            }

            return VerifyLoginCredential(new LoginModel { UserName = username, Password = pwd, Code = code, Line = line, FromLine = fromLine, TotalSeconds = totalSeconds }, rawPassword);
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
            var result = _userService.Login(
                model.UserName,
                rawPassword,
                    RequestHelper.GetRequestInfo(model.BrowserSize),
                out int custId,
                out int onuserId,
                out bool hasMultipleGroupsPermission);

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
                if (!string.IsNullOrWhiteSpace(model.FromLine))
                {
                    WriteLogSwitchLine($"Switched {model.FromLine} to {model.Line} failed: {ViewLang.AccessDenyToPublicDomain}, username: {model.UserName}", TraceEventType.Warning);
                }

                return View("~/Views/Account/Login.cshtml", new LoginModel(ViewLang.AccessDenyToPublicDomain));
            }

            FormsAuthentication.SetAuthCookie(model.UserName, false);

            SetUserSession(model, custId, onuserId, hasMultipleGroupsPermission, group);

            return RedirectOTPPage(model);
        }

        private void SetUMTokenForInternalAlphaLogin()
        {
            if (Request.QueryString[Config.InternalAgentUMToken] != null)
            {
                Session[Config.InternalAgentUMToken] = Request.QueryString[Config.InternalAgentUMToken];
            }
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectToRouteResult RedirectOTPPage(LoginModel model) => RedirectToAction("Index", "OTP", model);

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

            return RedirectToAction(LoginActionName);
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
            var result = _currentSession.IsITAdmin || _currentSession.IsInternalAdmin
                ? groupService.GetExternalGroup()
                : groupService.GetGroupWithSubGroups(_currentSession.GroupId);

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
            _currentSession.ParentGroupName = group.ParentGroupName == "None" ? group.GroupName : group.ParentGroupName;
            _currentSession.IsITAdmin = group.IsIT;
            const int CasinoExternalSystemCode = 3;
            _currentSession.FromSystem = CasinoExternalSystemCode;
            _currentSession.Captcha = model.Code;
            _currentSession.HashPassword = model.Password;
            _currentSession.CurrentLine = model.Line;
            _currentSession.UpdateLastActiveTime(_scLastActiveTime);
            int.TryParse(_scFailedCounter, out int failedCounter);
            _currentSession.SCFailedCounter = failedCounter;
        }

        private static void WriteLogSwitchLine(string message, System.Diagnostics.TraceEventType type)
        {
            if (ConfigurationManager.AppSettings["EnableWriteLogSwitchLine"] == "1")
            {
                ExceptionHandler.Handle(new Exception(message), type);
            }
        }
    }
}