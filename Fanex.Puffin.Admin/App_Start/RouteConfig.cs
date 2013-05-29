using System.Web.Mvc;

namespace Fanex.Puffin.Admin.App_Start
{
    public class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            context.MapRoute(
                areaName + "_Default",
                areaName + "/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.Puffin.Admin.Controllers" });
        }
    }
}