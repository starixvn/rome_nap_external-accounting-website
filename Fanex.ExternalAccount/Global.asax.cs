namespace Fanex.ExternalAccount
{
    using System;
    using System.Collections.Generic;
    using System.Web.Http;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Fanex.CustomerSetting.Client40;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.GoldDeluxe.CoreSetting.Mappings;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;

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
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = @"(.*/)?favicon.ico(/.*)?" });

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
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    custId = UrlParameter.Optional,
                    uplineId = UrlParameter.Optional,
                    username = UrlParameter.Optional,
                    groupname = UrlParameter.Optional
                });
        }

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            CustomerSettingRegistration.Init();
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            Fanex.Data.Configuration.DbSettingProviderManager.Current.Start();
            AutoMapperConfiguration.Configure();

            var logger = new CasinoAPILogger();
            var manager = new Sunplus.Libraries.APILogger.APILogManager(logger);
            string pattern = "AutoInsert*|Update*|Insert*|Move*|Reset*|FullSettlementResetMaxTransfer|Rollback*";
            manager.Register(typeof(CasinoLibrary.SBAgent.Customer), pattern);
            manager.Register(typeof(CasinoLibrary.SBAgent.BasicInfo), pattern);

            BundleTable.Bundles.EnableCommonBundle();
            BundleTable.Bundles.EnableLoginBundle();
            BundleTable.Bundles.EnableExternalGroupBundle();
            BundleTable.Bundles.EnableDailyWinLossBundle();
            BundleTable.Bundles.EnableStatementBundle();
            BundleTable.Bundles.EnableWinLossDetailBundle();

            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory = (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;

            MvcHandler.DisableMvcResponseHeader = true;

            ExceptionHandler
                .UseConfig(new LoggingConfiguration()
                .HideSensitiveInfo(GetRegexToHideSensitiveInfo())
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }

        private string RefreshUrlFactory(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action("Refresh", "DefaultCaptcha", new { area = string.Empty });
        }

        private string ImageUrlFactory(DefaultCaptchaManager captchaManager, UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            return urlHelper.Action(
            "Generate",
            "DefaultCaptcha",
            new RouteValueDictionary
            {
                { "area", string.Empty },
                { captchaManager.TokenParameterName, keyValuePair.Key }
            });
        }

        private static List<string> GetRegexToHideSensitiveInfo()
        {
            var result = new List<string>();
            var sensitiveInfo = new[] { "pass", "pwd", "security", "Password" };

            foreach (var info in sensitiveInfo)
            {
                result.Add($"(?:{info}=)(?<paramValue>[^&|^\\s]+)");
                result.Add($"(\"{info}\":)(?<paramValue>[^,|^\r]+)");
            }

            return result;
        }
    }
}