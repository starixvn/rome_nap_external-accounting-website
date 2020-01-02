namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using System.Web.Mvc;
    using Context;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ProductStatus.Setting.Services;

    public class ProductStatusAuthorizeService : IProductStatusAuthorizeService
    {
        private readonly ExternalAccountAuthenticateAttribute _authenticationService;
        private readonly AuthorizationService _authorization;

        public ProductStatusAuthorizeService()
            : this(null, null)
        {
        }

        public ProductStatusAuthorizeService(
            ExternalAccountAuthenticateAttribute authenticationService,
            AuthorizationService authorization)
        {
            _authenticationService = authenticationService ?? new ExternalAccountAuthenticateAttribute();
            _authorization = authorization ?? new AuthorizationServiceImpl();
        }

        public bool CanView(AuthorizationContext filterContext)
        {
            return HasPermission(filterContext, AuthenticateAction.AuthenticationType.Read);
        }

        public bool CanUpdate(AuthorizationContext filterContext)
        {
            return HasPermission(filterContext, AuthenticateAction.AuthenticationType.Host);
        }

        private bool HasPermission(AuthorizationContext filterContext, AuthenticateAction.AuthenticationType role)
        {
            var productIdParam = role == AuthenticateAction.AuthenticationType.Read
                ? filterContext.HttpContext.Request["productid"]
                : GetActionParameterValue(filterContext.Controller.ValueProvider, "productid");

            if (int.TryParse(productIdParam, out int productId))
            {
                var enableFunction = Enum.GetName(typeof(EnableStatusProductFunction), productId);
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
