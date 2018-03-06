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

        public ProductStatusAuthorizeService()
            : this(null)
        {
        }

        public ProductStatusAuthorizeService(ExternalAccountAuthenticateAttribute authenticationService)
        {
            _authenticationService = authenticationService ?? new ExternalAccountAuthenticateAttribute();
        }

        public bool CanView(AuthorizationContext filterContext)
        {
            int productId;

            if (int.TryParse(filterContext.HttpContext.Request["productid"], out productId))
            {
                return HasPermission(AuthenticateAction.AuthenticationType.Read, productId);
            }

            return false;
        }

        public bool CanUpdate(AuthorizationContext filterContext)
        {
            int productId;

            if (int.TryParse(filterContext.HttpContext.Request["productid"], out productId))
            {
                return HasPermission(AuthenticateAction.AuthenticationType.Host, productId);
            }

            return false;
        }

        public bool HasPermission(AuthenticateAction.AuthenticationType role, int productId)
        {
            var enableFunction = Enum.GetName(typeof(EnableStatusProductFunction), productId);

            return role == AuthenticateAction.AuthenticationType.Read
                ? _authenticationService.AllowReadByAuthenticationParam(enableFunction)
                : _authenticationService.AllowUpdateByAuthenticationParam(enableFunction);
        }
    }
}
