namespace Fanex.ExternalAccount.Core.Authorization
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Context;
    using CustomerSetting.Security.Authenticate;
    using DTO;
    using DTO.UserControl;
    using Fanex.ExternalAccount.Service;
    using Fanex.ExternalAccount.Service.Impl.UserControl;
    using Shared;

    public interface IAuthorizationService
    {
        bool IsBelong();

        bool IsBelong(string groupName, int custId, string userName);

        bool IsBelong(string groupName, IEnumerable<int> custIds);

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

    public class AuthorizationServiceImpl : IAuthorizationService
    {
        private static readonly string[] UrlToCheckUsernameParam = new[]
        {
            "NapStatement/StatementReport/GetCustomerInfo"
        };
        public static readonly AuthorizationServiceImpl Instance = new AuthorizationServiceImpl();

        private readonly ICachingService cachingService;
        private readonly IIPInformatonService ipInformationService;
        private readonly IBelongingService belongingService;
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static HttpContext? HttpContext
        {
            get
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    return _httpContextAccessor.HttpContext;
                }
                
                // Fallback
                try
                {
                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    var accessor = serviceProvider.GetService<IHttpContextAccessor>();
                    return accessor?.HttpContext;
                }
                catch
                {
                    return null;
                }
            }
        }

        public AuthorizationServiceImpl() : this(null, null, null)
        {
        }

        public AuthorizationServiceImpl(
            ICachingService cachingService,
            IIPInformatonService ipInformationService,
            IBelongingService belongingService)
        {
            this.cachingService = cachingService ?? new CachingService();
            this.ipInformationService = ipInformationService ?? new IPInformationService();
            this.belongingService = belongingService ?? new BelongingService();
        }

        public bool IsBelong()
        {
            var extRequest = ExtRequest.Instance;

            if (extRequest.CustIds.Any())
            {
                return IsBelong(extRequest.GetGroupName(), extRequest.CustIds);
            }

            var request = HttpContext?.Request;
            var url = request != null ? request.Path.Value?.TrimStart('/') ?? string.Empty : string.Empty;

            foreach (var item in UrlToCheckUsernameParam)
            {
                if (url.IndexOf(item, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    return IsBelong(extRequest.GetGroupName(), 0, extRequest.CustName);
                }
            }

            return IsBelong(extRequest.GetGroupName(), extRequest.CustId, string.Empty);
        }

        public bool IsBelong(string groupName, int custId, string userName)
        {
            var currentSession = CurrentSession.Instance;

            return currentSession.IsInternalAdmin
                || belongingService.IsBelonging(currentSession.UserId, groupName, custId, userName);
        }

        public bool IsBelong(string groupName, IEnumerable<int> custIds)
        {
            foreach (var id in custIds)
            {
                var isBeloging = IsBelong(groupName, id, string.Empty);
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
            var request = HttpContext?.Request;
            var requestUrl = string.Empty;

            if (request != null 
                && !request.Path.Value.Equals("/")
                && !request.Path.Value.ToLower(CultureInfo.InvariantCulture).Contains("home/index")
                && !request.Path.Value.ToLower(CultureInfo.InvariantCulture).Contains("exporttoexcel")
                && !isAjaxRequest)
            {
                requestUrl = request.Path.Value ?? string.Empty;
            }

            return IsAuthorized(userId, functionKey, authenticateActionType, requestUrl);
        }

        public bool IsAuthorized(
            int userId,
            string functionKey,
            AuthenticateAction.AuthenticationType policy = AuthenticateAction.AuthenticationType.Read,
            string requestUrl = "")
        {
            var currentSession = new CurrentSession();
            var userCache = cachingService.GetUserCache(userId, currentSession.ParentGroupName);
            var menuList = userCache.MenuList;
            var userFunctions = userCache.PolicyList.Where(x => x.FuncKey == functionKey).ToList();
            var noPermissionOnAnyMenu = !menuList.Any(menuItem =>
                IsAuthorizedUrl(menuItem.Url?.Replace("~", string.Empty), requestUrl));

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
            => IsAuthorized(CurrentSession.Instance.UserId, eFunctions.ViewVNIP) || !ipInformationService.IsSpecialIP(ip);
      
    }
}
