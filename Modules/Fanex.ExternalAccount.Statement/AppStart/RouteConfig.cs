namespace Fanex.ExternalAccount.Statement.AppStart
{
    using System.Reflection;
    using System.Web.Mvc;

    public class RouteConfig
    {
        public static void RegisterRoutes(string areaName, AreaRegistrationContext context)
        {
            var controllerName = Assembly.GetExecutingAssembly().GetName().Name + ".Controllers";
            context.MapRoute(
                areaName + "_Default",
                areaName + "/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional },
                new[] { controllerName });
        }
    }
}
