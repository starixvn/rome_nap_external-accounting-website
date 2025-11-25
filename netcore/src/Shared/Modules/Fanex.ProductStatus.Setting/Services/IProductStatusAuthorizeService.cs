namespace Fanex.ProductStatus.Setting.Services
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public interface IProductStatusAuthorizeService
    {
        bool CanView(AuthorizationFilterContext filterContext);

        bool CanUpdate(AuthorizationFilterContext filterContext);
    }
}
