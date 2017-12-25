namespace Fanex.ExternalAccount.Core.Context
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.UserControl;
    using Service.Impl.Login;
    using Service.Impl.Report;
    using Service.Impl.UserControl;
    using Service.Interface.UserControl;

    public class AuthorizationHandler
    {
        private readonly ICachingService _cachingService = new CachingService();
        public static readonly AuthorizationHandler Instance = new AuthorizationHandler();

        public bool IsBelong()
        {
            var currentContext = new CurrentContext();
            if (currentContext.CustIds.Any())
            {
                return IsBelong(CurrentContext.GroupName, currentContext.CustIds, string.Empty, true);
            }

            return IsBelong(CurrentContext.GroupName, currentContext.CustId, string.Empty, true);
        }

        public bool IsBelong(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            var currentSession = GetCurrentSession();

            if (currentSession.IsInternalAdmin)
            {
                return currentSession.IsInternalAdmin;
            }

            var loginService = new LoginService();
            var groups = loginService.GroupList();
            var userGroup = groups.Where(o => o.GroupID == currentSession.GroupId).ToList();
            groupname = string.IsNullOrEmpty(groupname) ? userGroup.FirstOrDefault()?.GroupName : groupname;
            var result = new ReportService().GetCustomerInfo(groupname, custId, userName, isGetUplineInfo);

            return result?.Rows.Count > 0 || (custId == 0 && userGroup.Count > 0);
        }

        private static bool IsBelong(string groupname, IEnumerable<int> custIds, string userName, bool isGetUplineInfo)
        {
            var currentSession = GetCurrentSession();

            if (currentSession.IsInternalAdmin)
            {
                return currentSession.IsInternalAdmin;
            }

            var loginService = new LoginService();
            var groups = loginService.GroupList();
            var userGroup = groups.Where(o => o.GroupID == currentSession.GroupId);
            groupname = string.IsNullOrEmpty(groupname) ? userGroup.FirstOrDefault()?.GroupName : groupname;

            if (groupname == null)
            {
                return true;
            }

            foreach (var id in custIds)
            {
                var result = new ReportService().GetCustomerInfo(groupname, id, userName, isGetUplineInfo);
                if (result == null || result.Rows.Count <= 0)
                {
                    return false;
                }
            }

            return true;
        }

        private static CurrentSession GetCurrentSession()
        {
            var currentSession = new CurrentSession();
            var loginService = new LoginService();
            var userInfo = new UserService().GetUserById(currentSession.UserId);

            if (currentSession.GroupId == userInfo.GroupID)
            {
                return currentSession;
            }

            currentSession.IsInternalAdmin =
                loginService.CheckExternalGroup(currentSession.UserId, 11, "ExternalGroup");
            currentSession.GroupId = userInfo.GroupID;
            currentSession.CurrencyGroup = userInfo.GroupName;
            currentSession.IsITAdmin = currentSession.GroupId == 1;

            return currentSession;
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
            AuthenticateAction.AuthenticationType policy,
            string requestUrl = "")
        {
            var userCache = _cachingService.GetUserCache(userId);
            var menuList = userCache.MenuList;
            var functions = userCache.PolicyList.Where(x => x.FuncKey == functionKey);
            var hasPerm = menuList.Any(menuItem => IsAuthorizedUrl(menuItem.Url.Replace("~", string.Empty), requestUrl));

            if (!hasPerm && functions == null)
            {
                return false;
            }

            if (!functions.Any())
            {
                return false;
            }

            return IsAuthorized(policy, functions.First());
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
                && (string.IsNullOrWhiteSpace(requestUrl) || requestUrl.ToLowerInvariant().Contains(menuUrl.ToLowerInvariant()));

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
            => functions.Any(function => IsAuthorized(userId, function, AuthenticateAction.AuthenticationType.Read));
    }
}