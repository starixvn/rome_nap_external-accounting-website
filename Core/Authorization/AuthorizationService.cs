using Fanex.CustomerSetting.IP.Utils;

namespace Fanex.ExternalAccount.Core.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Context;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.UserControl;
    using Service.Impl.Login;
    using Service.Impl.Report;
    using Service.Impl.UserControl;
    using Service.Interface.Login;
    using Service.Interface.Report;
    using Service.Interface.UserControl;

    public interface AuthorizationService
    {
        bool IsBelong();

        bool IsBelong(string groupName, int custId, string userName, bool isGetUplineInfo);

        bool IsAuthorized(
            int userId,
            string functionKey,
            AuthenticateAction.AuthenticationType authenticateActionType,
            bool isAjaxRequest);

        bool IsAuthorized(
            int userId,
            string functionKey,
            AuthenticateAction.AuthenticationType policy = AuthenticateAction.AuthenticationType.Read,
            string requestUrl = "");

        bool HasUpdateCreditPermission(int userId);

        bool HasReadPermissionOnFunctions(IEnumerable<string> functions, int userId);

        bool CanViewVNIP(string ip);
    }

    public class AuthorizationServiceImpl : AuthorizationService
    {
        public static readonly AuthorizationServiceImpl Instance = new AuthorizationServiceImpl();

        private readonly ICachingService cachingService;
        private readonly ILoginService loginService;
        private readonly IReportService reportService;
        private readonly IUserService userService;

        public AuthorizationServiceImpl() : this(null, null, null, null)
        {
        }

        public AuthorizationServiceImpl(
            ICachingService cachingService,
            ILoginService loginService,
            IReportService reportService,
            IUserService userService)
        {
            this.cachingService = cachingService ?? new CachingService();
            this.loginService = loginService ?? new LoginService();
            this.reportService = reportService ?? new ReportService();
            this.userService = userService ?? new UserService();
    }

        public bool IsBelong()
        {
            var currentContext = new CurrentContext();
            if (currentContext.CustIds.Any())
            {
                return IsBelong(CurrentSession.Instance.CurrencyGroup, currentContext.CustIds, string.Empty, true);
            }

            return IsBelong(CurrentSession.Instance.CurrencyGroup, currentContext.CustId, string.Empty, true);
        }

        public bool IsBelong(string groupName, int custId, string userName, bool isGetUplineInfo)
        {
            var currentSession = new CurrentSession();

            if (currentSession.IsInternalAdmin)
            {
                return true;
            }

            var groups = loginService.GroupList();
            var userGroup = groups.Where(o => o.GroupID == currentSession.GroupId).ToList();

            var defaultGroupName = string.IsNullOrEmpty(groupName) ? userGroup.FirstOrDefault()?.GroupName : groupName;

            var result = reportService.GetCustomerInfo(defaultGroupName, custId, userName, isGetUplineInfo);

            return result?.Rows.Count > 0 || custId == 0 && userGroup.Count > 0;
        }

        private bool IsBelong(string groupName, IEnumerable<int> custIds, string userName, bool isGetUplineInfo)
        {
            var currentSession = new CurrentSession();

            if (currentSession.IsInternalAdmin)
            {
                return true;
            }

            var groups = loginService.GroupList();
            var userGroup = groups.Where(o => o.GroupID == currentSession.GroupId).ToList();

            var defaultGroupName = string.IsNullOrEmpty(groupName) ? userGroup.FirstOrDefault()?.GroupName : groupName;

            if (defaultGroupName == null)
            {
                return true;
            }

            foreach (var id in custIds)
            {
                var result = reportService.GetCustomerInfo(defaultGroupName, id, userName, isGetUplineInfo);
                if (result == null || result.Rows.Count <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        public bool IsAuthorized(
            int userId,
            string functionKey,
            AuthenticateAction.AuthenticationType authenticateActionType,
            bool isAjaxRequest)
        {
            var request = HttpContext.Current.Request;
            var requestUrl = string.Empty;

            if (!request.RawUrl.Equals("/")
                && !request
                    .RawUrl
                    .ToLower(CultureInfo.InvariantCulture)
                    .Contains("home/index")
                && !request.RawUrl.ToLower(CultureInfo.InvariantCulture).Contains("exporttoexcel")
                && !isAjaxRequest)
            {
                requestUrl = request.RawUrl;
            }

            return IsAuthorized(userId, functionKey, authenticateActionType, requestUrl);
        }

        public bool IsAuthorized(
            int userId,
            string functionKey,
            AuthenticateAction.AuthenticationType policy = AuthenticateAction.AuthenticationType.Read,
            string requestUrl = "")
        {
            var userCache = cachingService.GetUserCache(userId);
            var menuList = userCache.MenuList;
            var userFunctions = userCache.PolicyList.Where(x => x.FuncKey == functionKey).ToList();
            var noPermissionOnAnyMenu = !menuList.Any(menuItem =>
                IsAuthorizedUrl(menuItem.Url.Replace("~", string.Empty), requestUrl));

            if (noPermissionOnAnyMenu && userFunctions.Count == 0)
            {
                return false;
            }

            if (userFunctions.Count == 0)
            {
                return false;
            }

            return IsAuthorized(policy, userFunctions[0]);
        }

        private static bool IsAuthorized(AuthenticateAction.AuthenticationType policy, Function function)
        {
            if (policy == AuthenticateAction.AuthenticationType.Read)
            {
                if (function.Policy == EPolicy.Read
                    || function.Policy == EPolicy.Update
                    || function.Policy == EPolicy.Delete
                    || function.Policy == EPolicy.Create)
                {
                    return true;
                }
            }
            else
            {
                return function.Policy != EPolicy.Read;
            }

            return false;
        }

        private static bool IsAuthorizedUrl(string menuUrl, string requestUrl)
            => !string.IsNullOrWhiteSpace(menuUrl)
               && (string.IsNullOrWhiteSpace(requestUrl)
               || requestUrl.IndexOf(menuUrl, System.StringComparison.InvariantCultureIgnoreCase) >= 0);

        public bool HasUpdateCreditPermission(int userId)
            => IsAuthorized(userId, eFunctions.UpdateCredit, AuthenticateAction.AuthenticationType.Host);

        public bool HasCloseCustomerPermission(int userId)
            => IsAuthorized(userId, eFunctions.CloseCustomer, AuthenticateAction.AuthenticationType.Host);

        public bool HasSuspendCustomerPermission(int userId)
            => IsAuthorized(userId, eFunctions.SuspendCustomer, AuthenticateAction.AuthenticationType.Host);

        public bool HasAllowAutoMPTPermission(int userId)
            => IsAuthorized(userId, eFunctions.AllowAutoMPT, AuthenticateAction.AuthenticationType.Host);

        public bool HasClearCreditPermission(int userId)
            => IsAuthorized(userId, eFunctions.ClearCredit, AuthenticateAction.AuthenticationType.Host);

        public bool HasOpenSuperPermission(int userId)
            => IsAuthorized(userId, eFunctions.OpenSuper, AuthenticateAction.AuthenticationType.Host);

        public bool HasReadPermissionOnFunctions(IEnumerable<string> functions, int userId)
            => functions.Any(function => IsAuthorized(userId, function));

        public bool CanViewVNIP(string ip)
            => !IpUtils.IsVnIp(ip) || IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP);
    }
}