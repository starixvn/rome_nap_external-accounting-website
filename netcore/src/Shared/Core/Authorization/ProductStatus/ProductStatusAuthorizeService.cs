namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;
    using Microsoft.AspNetCore.Mvc.Filters;

    // Stub implementation - external libraries not available
    public class ProductStatusAuthorizeService : IProductStatusAuthorizeService
    {
        public ProductStatusAuthorizeService()
        {
        }

        public ProductStatusAuthorizeService(object authenticationService, object authorization, object currentSession)
        {
            // Stub constructor
        }

        public bool CanView(AuthorizationFilterContext filterContext)
        {
            throw new NotImplementedException("ProductStatusAuthorizeService requires external ASP.NET MVC and Fanex.ProductStatus libraries");
        }

        public bool CanUpdate(AuthorizationFilterContext filterContext)
        {
            throw new NotImplementedException("ProductStatusAuthorizeService requires external ASP.NET MVC and Fanex.ProductStatus libraries");
        }
    }

    public interface IProductStatusAuthorizeService
    {
        bool CanView(AuthorizationFilterContext filterContext);
        bool CanUpdate(AuthorizationFilterContext filterContext);
    }
}