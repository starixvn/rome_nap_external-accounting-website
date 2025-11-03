namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Web.Mvc;
    using Context;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ProductStatus.Setting.Services;
    using CurrentSession = Context.CurrentSession;

    public class ProductStatusAuthorizeService : IProductStatusAuthorizeService
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService;
        private readonly AuthorizationService _authorization;
        private readonly CurrentSession _currentSession;
        private readonly int eSportId = 16;

        public ProductStatusAuthorizeService()
            : this(null, null, null)
        {
        }

        public ProductStatusAuthorizeService(
            ExternalAccountAuthenticateAttribute authenticationService,
            AuthorizationService authorization,
            CurrentSession currentSession)
        {
            _authenticationService = authenticationService ?? new ExternalAccountAuthenticateAttribute();
            _authorization = authorization ?? new AuthorizationServiceImpl();
            _currentSession = currentSession ?? new CurrentSession();
        }

        public bool CanView(AuthorizationContext filterContext)
            => HasPermission(filterContext, AuthenticateAction.AuthenticationType.Read);

        public bool CanUpdate(AuthorizationContext filterContext)
            => HasPermission(filterContext, AuthenticateAction.AuthenticationType.Host);

        private bool HasPermission(AuthorizationContext filterContext, AuthenticateAction.AuthenticationType role)
        {
            var productIdParam = role == AuthenticateAction.AuthenticationType.Read
                ? filterContext.HttpContext.Request["productid"]
                : GetActionParameterValue(filterContext.Controller.ValueProvider, "productid");

            var isGroupParam = role == AuthenticateAction.AuthenticationType.Read
                ? filterContext.HttpContext.Request["isGroup"]
                : GetActionParameterValue(filterContext.Controller.ValueProvider, "isGroup");

            bool.TryParse(isGroupParam, out bool isGroup);
            if (int.TryParse(productIdParam, out int productId))
            {
                if (_currentSession.IsBelongToSpecialGroups && productId != eSportId)
                {
                    return false;
                }

                var enableFunction = isGroup ? Enum.GetName(typeof(EnableStatusProductGroupFunction), productId) : Enum.GetName(typeof(EnableStatusProductFunction), productId);
                var hasPermission = role == AuthenticateAction.AuthenticationType.Read
                    ? _authenticationService.AllowReadByAuthenticationParam(enableFunction)
                    : _authenticationService.AllowUpdateByAuthenticationParam(enableFunction);

                return hasPermission && _authorization.IsBelong();
            }

            return false;
        }

        private static string GetActionParameterValue(IValueProvider valueProvider, string parameterName)
        {
            var valueProviderResult = valueProvider.GetValue(parameterName);

            return valueProviderResult != null
                ? valueProviderResult.AttemptedValue
                : string.Empty;
        }
    }
}