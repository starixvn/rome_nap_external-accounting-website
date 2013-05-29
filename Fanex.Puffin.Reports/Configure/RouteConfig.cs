using System.Web.Mvc;

namespace Fanex.Puffin.Report.Configure
{
    public class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            context.MapRoute(
                areaName + "_Default",
                "Report/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { "Fanex.Puffin.Report.Controllers" });
        }
    }
}