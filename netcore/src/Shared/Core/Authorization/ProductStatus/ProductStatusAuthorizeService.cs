namespace Fanex.ExternalAccount.Core.Authorization.ProductStatus
{
    using System;

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

        public bool CanView(object filterContext)
        {
            throw new NotImplementedException("ProductStatusAuthorizeService requires external ASP.NET MVC and Fanex.ProductStatus libraries");
        }

        public bool CanUpdate(object filterContext)
        {
            throw new NotImplementedException("ProductStatusAuthorizeService requires external ASP.NET MVC and Fanex.ProductStatus libraries");
        }
    }

    public interface IProductStatusAuthorizeService
    {
        bool CanView(object filterContext);
        bool CanUpdate(object filterContext);
    }
}