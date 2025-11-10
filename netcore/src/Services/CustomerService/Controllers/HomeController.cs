using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;
using Fanex.Caching;
using Fanex.ExternalAccount.Service.Impl.UserControl;
using Fanex.Logging.Extensions.Web;

namespace Fanex.ExternalAccount.Controllers.Home
{
    [ApiController]
    [Route("api/[controller]")]
    public class HomeController : ControllerBase
    {
        private readonly ICurrentSession _currentSession;
        private readonly IGroupService _groupService;
        private readonly ICacheService _cacheService;

        public HomeController(ICurrentSession currentSession, IGroupService groupService, ICacheService cacheService)
        {
            _currentSession = currentSession;
            _groupService = groupService;
            _cacheService = cacheService;
        }

        [HttpGet]
        [Route("Index/{id?}")]
        [Authorize] // TODO: Update with proper authorization policy
        public IActionResult Index(string? id)
        {
            if (_currentSession.IsInternalAdmin)
            {
                var groupName = id;

                if (!string.IsNullOrWhiteSpace(groupName)
                    && !groupName.Equals(_currentSession.CurrencyGroup)
                    && !IsIgnoreGroupName(groupName))
                {
                    var group = _groupService.GetGroupByName(groupName);

                    if (group != null)
                    {
                        _currentSession.CurrencyGroup = group.GroupName;
                        _currentSession.ParentGroupName = group.ParentGroupId == 0 ? group.GroupName : group.ParentGroupName;
                    }
                }
            }

            return Ok(new HomeViewModel
            {
                Username = _currentSession.UserName,
                Code = _currentSession.OTPEncryptCode,
                EnabledOTP = _currentSession.EnabledOTP,
                ModifiedDate = _currentSession.OTPModifiedDate
            });
        }

        private static bool IsIgnoreGroupName(string groupName)
            => groupName.IndexOf(".js.map", StringComparison.OrdinalIgnoreCase) >= 0;

        [HttpGet]
        [Route("TestLog")]
        public IActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));
            return Ok(new { message = "Test log executed" });
        }

        [HttpGet]
        [Route("TestExceptionLog")]
        [AllowAnonymous]
        public IActionResult TestExceptionLog()
        {
            throw new NotImplementedException("Test Exception.");
        }

        [HttpGet]
        [Route("TestSentryLog")]
        public IActionResult TestSentryLog()
        {
            return Ok(new { message = "Sentry test endpoint" });
        }

        [HttpGet]
        [Route("RedisGet")]
        public IActionResult RedisGet([FromQuery] string key)
        {
            var value = _cacheService.Get<string>(key);
            return Ok(value);
        }

        [HttpPost]
        [Route("RedisSet")]
        public IActionResult RedisSet([FromQuery] string key, [FromQuery] string value)
        {
            _cacheService.Set<string>(key, value);
            return Ok("OK");
        }
    }

    // TODO: Define proper interfaces and models
    public interface ICurrentSession
    {
        bool IsInternalAdmin { get; }
        bool IsLogin { get; }
        string CurrencyGroup { get; set; }
        string ParentGroupName { get; set; }
        string UserName { get; }
        string OTPEncryptCode { get; }
        bool EnabledOTP { get; }
        DateTime OTPModifiedDate { get; }
        int GroupId { get; }
        int UserId { get; }
        int CustId { get; }
        string CurrentLine { get; }
    }

    public class CurrentSession : ICurrentSession
    {
        // TODO: Implement actual session logic
        public bool IsInternalAdmin => false;
        public bool IsLogin => true; // TODO: Implement proper login check
        public string CurrencyGroup { get; set; } = string.Empty;
        public string ParentGroupName { get; set; } = string.Empty;
        public string UserName => "testuser";
        public string OTPEncryptCode => string.Empty;
        public bool EnabledOTP => false;
        public DateTime OTPModifiedDate => DateTime.Now;
        public int GroupId => 1; // TODO: Implement proper group ID
        public int UserId => 1; // TODO: Implement proper user ID
        public int CustId => 1; // TODO: Implement proper customer ID
        public string CurrentLine => "default"; // TODO: Implement proper current line
    }

    public class HomeViewModel
    {
        public string Username { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public bool EnabledOTP { get; set; }
        public DateTime ModifiedDate { get; set; }
    }
}