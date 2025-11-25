namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Core.Authorization;
    using Core.Base;
    using Core.Context;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Logging.Extensions.Web;
    using ViewModels;
    using CurrentSession = Core.Context.CurrentSession;

    public class HomeController : BaseController
    {
        private readonly ICurrentSession _currentSession;
        private readonly IGroupService _groupService;
        private readonly ICacheService _cacheService;

        public HomeController(
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            Core.Authorization.IAuthorizationService authorizationService,
            IGroupService groupService,
            ICacheService cacheService)
            : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            _currentSession = currentSession;
            _groupService = groupService;
            _cacheService = cacheService;
        }

        [ExternalAccountAuthenticate]
        public IActionResult Index()
        {
            if (CurrentSession.IsInternalAdmin)
            {
                var groupName = RouteData.Values["id"]?.ToString();

                if (!string.IsNullOrWhiteSpace(groupName)
                    && !groupName.Equals(CurrentSession.CurrencyGroup)
                    && !IsIgnoreGroupName(groupName))
                {
                    var group = _groupService.GetGroupByName(groupName);

                    if (group != null)
                    {
                        CurrentSession.CurrencyGroup = group.GroupName;
                        CurrentSession.ParentGroupName = group.ParentGroupId == 0 ? group.GroupName : group.ParentGroupName;
                    }
                }
            }

            return View(new HomeViewModel
            {
                Username = CurrentSession.UserName,
                Code = CurrentSession.OTPEncryptCode,
                EnabledOTP = CurrentSession.EnabledOTP,
                ModifiedDate = CurrentSession.OTPModifiedDate
            });
        }

        private static bool IsIgnoreGroupName(string groupName)
            => groupName.IndexOf(".js.map", StringComparison.OrdinalIgnoreCase) >= 0;

        public IActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return View();
        }

        [AllowAnonymous]
        public IActionResult TestExceptionLog()
        {
            throw new NotImplementedException("Test Exception.");
        }

        public IActionResult TestSentryLog()
            => View();

        public IActionResult RedisGet(string key)
        {
            var result = _cacheService.Get<string>(key);
            return Ok(result);
        }

        public IActionResult RedisSet(string key, string value)
        {
            _cacheService.Set<string>(key, value);
            return Ok("OK");
        }
    }
}