// --------------------------------------------------------------------------
// <copyright file="Global.asax.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.9.17">Created</change>
// </history>

namespace Fanex.ExternalAccount
{
    using System;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using Fanex.ExternalAccount.Utilities;

    /* Note: For instructions on enabling IIS6 or IIS7 classic mode,
     visit http://go.microsoft.com/?LinkId=9394801*/

    /// <summary>
    /// TODO: Global application.
    /// </summary>
    public class MvcApplication : System.Web.HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        /// <summary>
        /// Registers routes.
        /// </summary>
        /// <param name="routes">Routes collection.</param>
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
                defaults: new 
                { 
                    controller = "Home", action = "Index", id = UrlParameter.Optional, 
                    custId = UrlParameter.Optional, uplineId = UrlParameter.Optional, username = UrlParameter.Optional, groupname = UrlParameter.Optional 
                });
        }

        /// <summary>
        /// Application is starting.
        /// </summary>
        protected void Application_Start()
        {
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

        protected void Application_Error(object sender, EventArgs e)
        {
            if (Request.Headers["X-Requested-With"] != "XMLHttpRequest")
            {
                Response.Redirect("~/Shared/Error");
            }
        }
    }
}