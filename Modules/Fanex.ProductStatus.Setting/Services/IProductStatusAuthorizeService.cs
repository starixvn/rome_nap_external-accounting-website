namespace Fanex.ProductStatus.Setting.Services
{
    using System.Web.Mvc;

    public interface IProductStatusAuthorizeService
    {
        bool CanView(AuthorizationContext filterContext);

        bool CanUpdate(AuthorizationContext filterContext);
    }
}
