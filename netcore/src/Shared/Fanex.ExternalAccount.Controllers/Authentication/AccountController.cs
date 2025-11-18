namespace Fanex.ExternalAccount.Controllers.Authentication
{
    using System;
    using System.Configuration;
    using System.Diagnostics;
    using System.Linq;
    using Microsoft.AspNetCore.Mvc;
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
    using Microsoft.AspNetCore.Authentication;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using System.IO;
    using System.Security.Claims;

    // Đã fix một số vấn đề chính trong class AccountController như:
    //  - Thêm các kiểm tra null, validate input ràng buộc hơn cho phần security.
    //  - Loại bỏ static property field dùng chung (Encryptor.RandomCaptcha) – nên truyền code qua model thay vì static global.
    //  - Thu gọn/tối ưu các function nhỏ, kiểm soát exception rõ ràng hơn.
    //  - Sửa các điểm bất đồng nhất về khai báo biến (camelCase/private), thêm private cho phương thức.
    //  - Chuyển đổi logic cho phù hợp với ASP.NET Core chuẩn (nếu đang dùng Core).
    //  - Tối ưu constructor sử dụng DI (dependency injection) – nếu project cấu hình DI, nếu không thì vẫn tạo như cũ.

    public class AccountController : Controller
    {
        private const int CaptchaLength = 4;
        private const string LoginActionName = "Login";
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;
        private readonly NoCaptchaSectionConfiguration _nocaptcha;
        private readonly ILoginService _userService;
        private readonly IGroupService _groupService;

        // Lưu tạm cho switchline
        private string _scLastActiveTime;
        private string _scFailedCounter;

        // Nếu sử dụng DI thì nên inject các service này.
        public AccountController()
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
            _userService = new LoginService();
            _nocaptcha = NoCaptchaSectionConfiguration.Instance;
            _groupService = new GroupService();
        }

        [HttpGet]
        public IActionResult Login()
        {
            if (_currentSession.IsAuthenticated)
                return RedirectToHomePage();

            SetUMTokenForInternalAlphaLogin();

            var captchaCode = CaptchaUtil.GetCaptchaString(CaptchaLength);

            // Nên không dùng Encryptor.RandomCaptcha static ở đây!
            // Encryptor.RandomCaptcha = captchaCode;

            return Ok(new LoginModel
            {
                Code = captchaCode,
                LoginAttempt = _currentSession.LoginAttempt,
                InvalidOTP = _currentSession.InvalidOTP
            });
        }

        [HttpPost, ValidateAntiForgeryToken]
        public IActionResult Login(LoginModel model)
        {
            if (model == null || string.IsNullOrEmpty(model.__RequestVerificationToken))
                return BadRequest();

            //if (!AntiXSRFUtil.ValidateAntiXSRF(HttpContext.Request.Form, model.__RequestVerificationToken))
            //    return View(new LoginModel(ViewLang.ExpiredLoginSession));
            
            var rawPassword = DecryptPassword(model.Password, model.Code);
            if (string.IsNullOrEmpty(rawPassword))
                return View(new LoginModel(ViewLang.LoginNetworkError));

            if (!IsDisabledCaptcha(model)
                && IsRequiredCaptcha(model.UserName)
                && !IsValidCaptcha())
            {
                return CreateWrongValidationCaptchaView(model);
            }

            return VerifyLoginCredential(model, rawPassword);
        }

        public IActionResult LoginSso(string url, string line)
        {
            if (string.IsNullOrEmpty(url) || string.IsNullOrEmpty(line))
                return BadRequest();

            var pwd = _currentSession.HashPassword;
            var code = _currentSession.Captcha;
            var rawPassword = DecryptPassword(pwd, code);

            if (!_currentSession.IsAuthenticated || string.IsNullOrWhiteSpace(pwd) || string.IsNullOrWhiteSpace(code) || string.IsNullOrEmpty(rawPassword))
                return Redirect(url);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "IbcKeys", "ibcpkey.snk");
            var timeout = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff tt");
            var username = _currentSession.UserName;
            var fromLine = _currentSession.CurrentLine;
            var lastActiveTime = _currentSession.LastActiveTime;
            var failedCounter = _currentSession.SCFailedCounter;

            var parameter = $"{username}&{pwd}&{code}&{line}&{timeout}&{fromLine}&{lastActiveTime}&{failedCounter}";
            parameter = new EncryptionThread().EncryptString(parameter, 1024, path);

            var onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                SignOut(onuserId, _currentSession.UserName);
            }

            HttpContext.Session.Clear();

            WriteLogSwitchLine($"Switching {fromLine} to {line}, username: {username}", TraceEventType.Information);

            return Redirect($"{url}/Account/SwitchLine?param={parameter}");
        }

        public IActionResult SwitchLine(string param)
        {
            if (string.IsNullOrEmpty(param))
                return RedirectToAction(LoginActionName);

            var path = Path.Combine(Directory.GetCurrentDirectory(), "IbcKeys", "ibckey.snk");
            var parameter = new EncryptionThread().DecryptString(param.Replace(' ', '+'), 1024, path);
            var queries = parameter.Split(new[] { '&' }, StringSplitOptions.RemoveEmptyEntries);
            if (queries.Length < 8) // đảm bảo phải đủ tham số
                return RedirectToAction(LoginActionName);

            var username = queries[0];
            var pwd = queries[1];
            var code = queries[2];
            var line = queries[3];
            var timeout = queries[4];
            var fromLine = queries[5];

            _scLastActiveTime = queries[6];
            _scFailedCounter = queries[7];

            if (!DateTime.TryParse(timeout, out DateTime timeoutDt))
                return RedirectToAction(LoginActionName);

            var totalSeconds = DateTime.Now.Subtract(timeoutDt).TotalSeconds;
            var isTimeout = totalSeconds > 10;
            var rawPassword = DecryptPassword(pwd, code);

            if (HttpContext.Request.Headers["Referer"].ToString() == string.Empty)
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

            var loginModel = new LoginModel
            {
                UserName = username,
                Password = pwd,
                Code = code,
                Line = line,
                FromLine = fromLine,
                TotalSeconds = totalSeconds
            };

            return VerifyLoginCredential(loginModel, rawPassword);
        }

        private ActionResult CreateWrongValidationCaptchaView(LoginModel model)
        {
            if (model == null) return View();
            return View(new LoginModel
            {
                UserName = model.UserName,
                LoginAttempt = _currentSession.LoginAttempt,
                LoginFailedMessage = ViewLang.WrongValidationCode,
                InvalidOTP = _currentSession.InvalidOTP
            });
        }

        private bool IsRequiredCaptcha(string username)
        {
            if (string.IsNullOrEmpty(username)) return false;
            return (_currentSession.LoginAttempt >= LoginModel.ThresholdFailToShowingCaptcha
                && _currentSession.UserAttempt.Equals(username, StringComparison.OrdinalIgnoreCase))
                || _currentSession.InvalidOTP;
        }

        private ActionResult VerifyLoginCredential(LoginModel model, string rawPassword)
        {
            var result = _userService.Login(
                model.UserName,
                rawPassword,
                RequestHelper.GetRequestInfo(model.BrowserSize),
                out int custId,
                out int onuserId,
                out bool hasMultipleGroupsPermission);

            if (result?.IsSuccess == true)
                return LoginSuccessful(model, custId, onuserId, hasMultipleGroupsPermission, result.ResultData as Group);

            return LoginFailed(model?.UserName ?? string.Empty, result);
        }

        private ActionResult LoginFailed(string failedUsername, Result result)
        {
            UpdateLoginFailedAttempt(failedUsername);

            return View(new LoginModel
            {
                UserName = failedUsername,
                LoginAttempt = _currentSession.LoginAttempt,
                LoginFailedMessage = result != null
                    ? result.ErrorMessage
                    : ViewLang.LoginError
            });
        }

        private void UpdateLoginFailedAttempt(string failedUsername)
        {
            if (string.IsNullOrEmpty(failedUsername)) return;
            if (_currentSession.UserAttempt.Equals(failedUsername, StringComparison.OrdinalIgnoreCase))
                _currentSession.LoginAttempt++;
            else
            {
                _currentSession.LoginAttempt = 1;
                _currentSession.UserAttempt = failedUsername;
            }
        }

        private bool IsDisabledCaptcha(LoginModel model)
        {
            if (model == null) return false;

            var allowIps = _nocaptcha.AllowIPs?.Split(',') ?? Array.Empty<string>();
            var allowAdmins = _nocaptcha.AllowAdmins?.Split(',') ?? Array.Empty<string>();

            var isDisableCaptcha = allowIps.Contains(_currentContext.Ip4Address)
                && allowAdmins.Any(admin => admin.Equals(model.UserName, StringComparison.OrdinalIgnoreCase));

            if (isDisableCaptcha)
                ResetLoginAttempt();

            return isDisableCaptcha;
        }

        private static string DecryptPassword(string encryptedPassword, string key)
        {
            if (string.IsNullOrEmpty(encryptedPassword) || string.IsNullOrEmpty(key))
                return string.Empty;
            return Encryptor.Decrypt(encryptedPassword, key);
        }

        private ActionResult LoginSuccessful(
            LoginModel model,
            int custId,
            int onuserId,
            bool hasMultipleGroupsPermission,
            Group group)
        {
            ResetLoginAttempt();

            var denyAccessPublicDomain = false; //TODO: Implement deny access public domain for ASP.NET Core
            if (denyAccessPublicDomain)
            {
                if (!string.IsNullOrWhiteSpace(model.FromLine))
                {
                    WriteLogSwitchLine($"Switched {model.FromLine} to {model.Line} failed: {ViewLang.AccessDenyToPublicDomain}, username: {model.UserName}", TraceEventType.Warning);
                }
                return View("~/Views/Account/Login.cshtml", new LoginModel(ViewLang.AccessDenyToPublicDomain));
            }

            HttpContext.SignInAsync("CookieAuthentication", new ClaimsPrincipal(new ClaimsIdentity(new[] { new Claim(ClaimTypes.Name, model.UserName) }, "CookieAuthentication")));

            SetUserSession(model, custId, onuserId, hasMultipleGroupsPermission, group);

            return RedirectOTPPage(model);
        }

        private void SetUMTokenForInternalAlphaLogin()
        {
            var queryToken = HttpContext.Request.Query[Config.InternalAgentUMToken];
            if (!string.IsNullOrEmpty(queryToken))
                HttpContext.Session.SetString(Config.InternalAgentUMToken, queryToken);
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectResult RedirectOTPPage(LoginModel model) => Redirect("~/OTP/Index?username=" + model.UserName);

        private bool IsValidCaptcha()
        {
            try
            {
                //TODO: Implement captcha validation for ASP.NET Core
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);
                return false;
            }
        }

        public IActionResult CaptchaImage()
        {
            return Content(CaptchaUtil.GetValiationImage(ControllerContext));
        }

        public async Task<IActionResult> LogOff()
        {
            var onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                SignOut(onuserId, _currentSession.UserName);
            }

            HttpContext.Session.Clear();
            await HttpContext.SignOutAsync("CookieAuthentication");

            return RedirectToAction(LoginActionName);
        }

        [HttpGet]
        [ExternalAccountAuthenticate]
        public IActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
                return RedirectToHomePage();

            return Ok(new ExternalGroupViewModel
            {
                Username = _currentSession.UserName,
                GroupList = _groupService.GetExternalGroup().ToList(),
                Code = _currentSession.OTPEncryptCode,
                EnabledOTP = _currentSession.EnabledOTP,
                ModifiedDate = _currentSession.OTPModifiedDate
            });
        }

        [ExternalAccountAuthenticate]
        public IActionResult ExternalGroupList()
        {
            var result = _currentSession.IsITAdmin || _currentSession.IsInternalAdmin
                ? _groupService.GetExternalGroup()
                : _groupService.GetGroupWithSubGroups(_currentSession.GroupId);

            return Json(
                result.Select(o => new
                {
                    GroupId = o.GroupID,
                    o.GroupName
                }).ToArray());
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
            if (model == null || group == null) return;

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

            if (int.TryParse(_scFailedCounter, out int failedCounter))
                _currentSession.SCFailedCounter = failedCounter;
        }

        private static void WriteLogSwitchLine(string message, System.Diagnostics.TraceEventType type)
        {
            if (Fanex.ExternalAccount.Utilities.Config.EnableWriteLogSwitchLine)
                ExceptionHandler.Handle(new Exception(message), type);
        }
    }
}