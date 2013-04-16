using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Fanex.ExternalAccount.Utilities;

namespace Fanex.ExternalAccount
{
    // Note: For instructions on enabling IIS6 or IIS7 classic mode,
    // visit http://go.microsoft.com/?LinkId=9394801

    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");

            routes.MapHttpRoute(
                name: "DefaultApi",
                routeTemplate: "api/{controller}/{id}",
                defaults: new { id = RouteParameter.Optional });

            routes.MapRoute(
                name: "Default",
                url: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default2",
                url: "{controller}/{action}/{id}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, groupname = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default3",
                url: "{controller}/{action}/{id}/{username}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, username = UrlParameter.Optional, groupname = UrlParameter.Optional });

            routes.MapRoute(
                name: "Default4",
                url: "{controller}/{action}/{id}/{custId}/{uplineId}/{username}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = UrlParameter.Optional, custId = UrlParameter.Optional, uplineId = UrlParameter.Optional, username = UrlParameter.Optional, groupname = UrlParameter.Optional });
        }

        protected void Application_Start()
        {
            Sunplus.Agent.Configurations.AppSettings.LoadComplete += new Sunplus.Agent.Configurations.AppSettings.UpdateEventHandler(AppSettings_LoadComplete);
            Sunplus.Agent.Configurations.AppSettings.Configuration.Load();

            AreaRegistration.RegisterAllAreas();

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);

            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();

            BundleTable.Bundles.EnableCommonBundle();
            BundleTable.Bundles.EnableLoginBundle();
            BundleTable.Bundles.EnableExternalGroupBundle();
            BundleTable.Bundles.EnableDailyWinLossBundle();
            BundleTable.Bundles.EnableStatementBundle();
            BundleTable.Bundles.EnableWinLossDetailBundle();
        }

        private void AppSettings_LoadComplete(object sender, bool error)
        {
            var keys = Sunplus.Agent.Configurations.AppSettings.Configuration.GetKeys();

            foreach (string key in keys)
            {
                ConfigurationManager.AppSettings[key] = Sunplus.Agent.Configurations.AppSettings.Configuration.Get(key);
            }
        }
    }
}