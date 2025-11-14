namespace Fanex.ProductStatus.Setting.Services
{
    using Microsoft.AspNetCore.Mvc;

    public interface IProductStatusAuthorizeService
    {
        bool CanView(AuthorizationContext filterContext);

        bool CanUpdate(AuthorizationContext filterContext);
    }
}
