namespace Fanex.ExternalAccount.Core.Authorization
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web;
    using Context;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.UserControl;
    using Service;
    using Service.Impl.Report;
    using Service.Impl.UserControl;
    using Service.Interface.Report;
    using Service.Interface.UserControl;
    using Shared;

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
        private readonly IReportService reportService;
        private readonly IIPInformatonService ipInformationService;

        public AuthorizationServiceImpl() : this(null, null, null)
        {
        }

        public AuthorizationServiceImpl(
            ICachingService cachingService,
            IReportService reportService,
            IIPInformatonService ipInformationService)
        {
            this.cachingService = cachingService ?? new CachingService();
            this.reportService = reportService ?? new ReportService();
            this.ipInformationService = ipInformationService ?? new IPInformationService();
        }

        public bool IsBelong()
        {
            if (ExtRequest.Instance.CustIds.Any())
            {
                return IsBelong(ExtRequest.Instance.GroupName, ExtRequest.Instance.CustIds);
            }

            return IsBelong(ExtRequest.Instance.GroupName, ExtRequest.Instance.CustId, string.Empty, false);
        }

        public bool IsBelong(string groupName, int custId, string userName, bool isGetUplineInfo)
        {
            if (!CurrentSession.Instance.IsInternalAdmin)
            {
                if (!string.IsNullOrEmpty(groupName) && !groupName.Equals(CurrentSession.Instance.CurrencyGroup))
                {
                    return false;
                }

                if (custId == 0 && string.IsNullOrEmpty(userName))
                {
                    return true;
                }

                var result = reportService.GetCustomerInfo(CurrentSession.Instance.CurrencyGroup, custId, userName, isGetUplineInfo);

                return result?.Rows.Count > 0;
            }

            return CurrentSession.Instance.IsInternalAdmin;
        }

        private bool IsBelong(string groupName, IEnumerable<int> custIds)
        {
            foreach (var id in custIds)
            {
                var isBeloging = IsBelong(groupName, id, string.Empty, false);
                if (!isBeloging)
                {
                    return isBeloging;
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
            => IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP) || !ipInformationService.Is789IP(ip);
    }
}