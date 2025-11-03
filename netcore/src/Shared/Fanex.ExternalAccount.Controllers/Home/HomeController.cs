namespace Fanex.ExternalAccount.Controllers.Home
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Logging.Extensions.Web;
    using ViewModels;
    using CurrentSession = Core.Context.CurrentSession;

    public class HomeController : BaseController
    {
        private readonly CurrentSession _currentSession;
        private readonly IGroupService _groupService;
        private readonly ICacheService _cacheService;

        public HomeController()
        {
            _currentSession = new CurrentSession();
            _groupService = new GroupService();
            _cacheService = new CacheService();
        }

        [ExternalAccountAuthenticate]
        public ActionResult Index()
        {
            if (_currentSession.IsInternalAdmin)
            {
                var groupName = (string)RouteData.Values["id"];

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

            return View(new HomeViewModel
            {
                Username = _currentSession.UserName,
                Code = _currentSession.OTPEncryptCode,
                EnabledOTP = _currentSession.EnabledOTP,
                ModifiedDate = _currentSession.OTPModifiedDate
            });
        }

        private static bool IsIgnoreGroupName(string groupName)
            => groupName.IndexOf(".js.map", StringComparison.OrdinalIgnoreCase) >= 0;

        public ActionResult TestLog()
        {
            ExceptionHandler.Handle(new Exception("Testing log"));

            return View();
        }

        [AllowAnonymous]
        public ActionResult TestExceptionLog()
        {
            throw new NotImplementedException("Test Exception.");
        }

        public ActionResult TestSentryLog()
            => View();

        public ActionResult RedisGet(string key)
        {
            return Json(_cacheService.Get<string>(key), JsonRequestBehavior.AllowGet);
        }

        public ActionResult RedisSet(string key, string value)
        {
            _cacheService.Set<string>(key, value);
            return Json("OK", JsonRequestBehavior.AllowGet);
        }
    }
}