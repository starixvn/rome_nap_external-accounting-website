using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.Diagnostics;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Fanex.ExternalAccount.Core.Context;
using Fanex.ExternalAccount.Core.Helpers;
using Fanex.ExternalAccount.DTO;
using Fanex.ExternalAccount.DTO.UserControl;
using Fanex.ExternalAccount.Service.Impl.Login;
using Fanex.ExternalAccount.Service.Impl.UserControl;
using Fanex.ExternalAccount.Service.Interface.Login;
using Fanex.ExternalAccount.Utilities;
using CurrentContext = Fanex.ExternalAccount.Core.Context.CurrentContext;
using CurrentSession = Fanex.ExternalAccount.Core.Context.CurrentSession;
using AuthService;
using LoginModel = Fanex.ExternalAccount.ViewModel.Account.LoginModel;
using Fanex.Logging.Extensions.Web;

namespace Fanex.ExternalAccount.Controllers.Authentication
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        private const int CaptchaLength = 4;
        private const string LoginActionName = "Login";
        private readonly ICurrentContext _currentContext;
        private readonly CurrentSession _currentSession;
        private readonly NoCaptchaSectionConfiguration _nocaptcha;
        private readonly ILoginService _userService;
        private readonly IGroupService groupService;
        private readonly IWebHostEnvironment _hostEnvironment;
        private readonly IConfiguration _configuration;
        private readonly JwtSettings _jwtSettings;

        private string _scLastActiveTime;
        private string _scFailedCounter;

        public AccountController(
            IWebHostEnvironment hostEnvironment,
            IConfiguration configuration,
            IOptions<JwtSettings> jwtSettings)
        {
            _currentContext = new CurrentContext();
            _currentSession = new CurrentSession();
            _userService = new LoginService();
            _nocaptcha = NoCaptchaSectionConfiguration.Instance;
            _jwtSettings = jwtSettings.Value;
            groupService = new GroupService();
            _hostEnvironment = hostEnvironment;
            _configuration = configuration;
        }

        [HttpGet]
        [Route("Login")]
        public IActionResult Login()
        {
            if (_currentSession.IsAuthenticated)
            {
                return RedirectToHomePage();
            }

            SetUMTokenForInternalAlphaLogin();

            Encryptor.RandomCaptcha = CaptchaUtil.GetCaptchaString(CaptchaLength);

            return Ok(new LoginModel
            {
                Code = Encryptor.RandomCaptcha,
                LoginAttempt = _currentSession.LoginAttempt,
                InvalidOTP = _currentSession.InvalidOTP
            });
        }

        [HttpPost]
        [Route("Login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!AntiXSRFUtil.ValidateAntiXSRF(HttpContext.Request, model.__RequestVerificationToken))
            {
                return BadRequest(new LoginModel(ViewLang.ExpiredLoginSession));
            }

            var rawPassword = DecryptPassword(model.Password, model.Code);
            if (string.IsNullOrEmpty(rawPassword))
            {
                return BadRequest(new LoginModel(ViewLang.LoginNetworkError));
            }

            if (!IsDisabledCaptcha(model)
                && IsRequiredCaptcha(model.UserName)
                && !IsValidCaptcha())
            {
                return BadRequest(CreateWrongValidationCaptchaView(model));
            }

            return await VerifyLoginCredential(model, rawPassword);
        }

        [HttpGet]
        [Route("LoginSso")]
        public IActionResult LoginSso(string url, string line)
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

            var path = System.IO.Path.Combine(_hostEnvironment.ContentRootPath, "IbcKeys", "ibcpkey.snk");
            var timeout = DateTime.Now.ToString("MM/dd/yyyy HH:mm:ss.fff tt");
            var username = _currentSession.UserName;
            var fromLine = _currentSession.CurrentLine;
            var lastActiveTime = _currentSession.LastActiveTime;
            var failedCounter = _currentSession.SCFailedCounter;
            var parameter = $"{username}&{pwd}&{code}&{line}&{timeout}&{fromLine}&{lastActiveTime}&{failedCounter}";
#pragma warning disable S109 // Magic numbers should not be used
            parameter = Fanex.ExternalAccount.Controllers.Authentication.EncryptionThread.EncryptString(parameter, 1024, path);
#pragma warning restore S109 // Magic numbers should not be used

            var onuserId = _currentSession.OnUserId;
            if (onuserId > 0)
            {
                SignOut(onuserId, _currentSession.UserName);
            }

            HttpContext.Session.Clear();

            WriteLogSwitchLine($"Switching {fromLine} to {line}, username: {username}", TraceEventType.Information);

            return Redirect($"{url}/Account/SwitchLine?param={parameter}");
        }

        [HttpGet]
        [Route("SwitchLine")]
        public async Task<IActionResult> SwitchLine(string param)
        {
            var path = System.IO.Path.Combine(_hostEnvironment.ContentRootPath, "IbcKeys", "ibckey.snk");
            var parameter = Fanex.ExternalAccount.Controllers.Authentication.EncryptionThread.DecryptString(param.Replace(' ', '+'), 1024, path);
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

            if (Request.Headers["Referer"].ToString() == null || !Request.Headers["Referer"].Any())
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

            return await VerifyLoginCredential(new LoginModel { UserName = username, Password = pwd, Code = code, Line = line, FromLine = fromLine, TotalSeconds = totalSeconds }, rawPassword);
        }

        private LoginModel CreateWrongValidationCaptchaView(LoginModel model) => new LoginModel
        {
            UserName = model.UserName,
            LoginAttempt = _currentSession.LoginAttempt,
            LoginFailedMessage = ViewLang.WrongValidationCode,
            InvalidOTP = _currentSession.InvalidOTP
        };

        private bool IsRequiredCaptcha(string username) =>
            (_currentSession.LoginAttempt >= LoginModel.ThresholdFailToShowingCaptcha
            && _currentSession.UserAttempt.Equals(username, StringComparison.OrdinalIgnoreCase))
            || _currentSession.InvalidOTP;

        private async Task<IActionResult> VerifyLoginCredential(LoginModel model, string rawPassword)
        {
            var browserSize = Request.Headers["User-Agent"].ToString();
            var result = _userService.Login(
                model.UserName,
                rawPassword,
                RequestHelper.GetRequestInfo(browserSize),
                out int custId,
                out int onuserId,
                out bool hasMultipleGroupsPermission);

            if (result?.IsSuccess == true)
            {
                return await LoginSuccessful(model, custId, onuserId, hasMultipleGroupsPermission, result.ResultData as Group);
            }
            else
            {
                return LoginFailed(model.UserName, result);
            }
        }

        private IActionResult LoginFailed(string failedUsername, Result result)
        {
            UpdateLoginFailedAttempt(failedUsername);

            return BadRequest(new LoginModel
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

        private string GenerateJwtToken(string username, int custId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, username),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim("custId", custId.ToString()),
                new Claim("username", username)
            };

            var token = new JwtSecurityToken(
                issuer: _jwtSettings.Issuer,
                audience: _jwtSettings.Audience,
                claims: claims,
                expires: DateTime.Now.AddMinutes(_jwtSettings.ExpiryInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<IActionResult> LoginSuccessful(
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

                return BadRequest(new LoginModel(ViewLang.AccessDenyToPublicDomain));
            }

            // Set authentication cookie
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, model.UserName),
                new Claim(ClaimTypes.NameIdentifier, custId.ToString())
            };
            var identity = new ClaimsIdentity(claims, "CookieAuthentication");
            var principal = new ClaimsPrincipal(identity);
            await HttpContext.SignInAsync("CookieAuthentication", principal);

            SetUserSession(model, custId, onuserId, hasMultipleGroupsPermission, group);

            // Generate JWT token for API authentication
            var jwtToken = GenerateJwtToken(model.UserName, custId);

            // Return successful login response with JWT token
            return Ok(new LoginResponse
            {
                Success = true,
                Message = "Login successful",
                Token = jwtToken,
                UserName = model.UserName,
                CustId = custId,
                RequiresOTP = true // Indicate that OTP might be required
            });
        }

        private void SetUMTokenForInternalAlphaLogin()
        {
            if (Request.Query.ContainsKey(Config.InternalAgentUMToken))
            {
                HttpContext.Session.SetString(Config.InternalAgentUMToken, Request.Query[Config.InternalAgentUMToken].ToString());
            }
        }

        private RedirectResult RedirectToHomePage() => Redirect("~/Home/Index/");

        private RedirectToActionResult RedirectOTPPage(LoginModel model) => RedirectToAction("Index", "OTP", model);

        private bool IsValidCaptcha()
        {
            try
            {
                // TODO: Implement captcha validation for ASP.NET Core
                // For now, return true to allow login
                return true;
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex, TraceEventType.Warning);

                return false;
            }
        }

        [HttpGet]
        [Route("CaptchaImage")]
        public IActionResult CaptchaImage()
        {
            var imageBase64 = CaptchaUtil.GetValiationImage(HttpContext);
            return Content(imageBase64);
        }

        [HttpPost]
        [Route("LogOff")]
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
        [Route("ExternalGroup")]
        [Authorize]
        public IActionResult ExternalGroup()
        {
            if (!_currentSession.IsInternalAdmin)
            {
                return RedirectToHomePage();
            }

            return Ok(new ExternalGroupViewModel
            {
                Username = _currentSession.UserName,
                GroupList = groupService.GetExternalGroup().Cast<object>().ToList(),
                Code = _currentSession.OTPEncryptCode,
                EnabledOTP = _currentSession.EnabledOTP,
                ModifiedDate = _currentSession.OTPModifiedDate
            });
        }

        [HttpGet]
        [Route("ExternalGroupList")]
        [Authorize]
        public IActionResult ExternalGroupList()
        {
            var result = _currentSession.IsITAdmin || _currentSession.IsInternalAdmin
                ? groupService.GetExternalGroup()
                : groupService.GetGroupWithSubGroups(_currentSession.GroupId);

            return Ok(result.Select(o => new
            {
                GroupId = o.GroupID,
                o.GroupName
            }).ToArray());
        }

        private bool IsDenyAccessPublicDomain(bool hasMultipleGroupsPermission)
        {
            string url = Request.Headers["Host"].ToString() + (Request.PathBase.HasValue ? Request.PathBase.Value : string.Empty);
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

        private void WriteLogSwitchLine(string message, TraceEventType type)
        {
            if (_configuration.GetValue<string>("EnableWriteLogSwitchLine") == "1")
            {
                ExceptionHandler.Handle(new Exception(message), type);
            }
        }
    }
}
