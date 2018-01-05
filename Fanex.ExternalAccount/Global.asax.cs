using System.Configuration;
using System.Web.Configuration;
using System.Web.Security;

namespace Fanex.ExternalAccount
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Optimization;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using CasinoLibrary.SBAgent;
    using Core.Context;
    using CustomerSetting.Client40;
    using Data.Configuration;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;
    using GoldDeluxe.CoreSetting.Mappings;
    using ONEbook.UM.Filters;
    using Sunplus.Libraries.APILogger;
    using Utilities;

    public class MvcApplication : HttpApplication
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = "(.*/)?favicon.ico(/.*)?" });

            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });

            routes.MapRoute(
                "Default2",
                "{controller}/{action}/{id}/{groupname}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    groupname = UrlParameter.Optional
                });

            routes.MapRoute(
                "Default3",
                "{controller}/{action}/{id}/{username}/{groupname}",
                new
                {
                    controller = "Home",
                    action = "Index",
                    id = UrlParameter.Optional,
                    username = UrlParameter.Optional,
                    groupname = UrlParameter.Optional
                });

            routes.MapRoute(
                "Default4",
                "{controller}/{action}/{id}/{custId}/{uplineId}/{username}/{groupname}",
                new
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
            SetDynamicAuthenticationCookieName();

            AreaRegistration.RegisterAllAreas();
            CustomerSettingRegistration.Init();

            GlobalFilters.Filters.Add(new CheckUMFilter());
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DbSettingProviderManager.Current.Start();
            AutoMapperConfiguration.Configure();

            var logger = new CasinoAPILogger();
            var manager = new APILogManager(logger);
            const string pattern = "AutoInsert*|Update*|Insert*|Move*|Reset*|FullSettlementResetMaxTransfer|Rollback*";
            manager.Register(typeof(Customer), pattern);
            manager.Register(typeof(BasicInfo), pattern);

            BundleTable.Bundles.EnableCommonBundle();
            BundleTable.Bundles.EnableLoginBundle();
            BundleTable.Bundles.EnableExternalGroupBundle();
            BundleTable.Bundles.EnableDailyWinLossBundle();
            BundleTable.Bundles.EnableStatementBundle();
            BundleTable.Bundles.EnableWinLossDetailBundle();

            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory =
                (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
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

        private static string RefreshUrlFactory(UrlHelper urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair) =>
            urlHelper.Action("Refresh", "DefaultCaptcha", new { area = string.Empty });

        private static string ImageUrlFactory(DefaultCaptchaManager captchaManager, UrlHelper urlHelper,
            KeyValuePair<string, ICaptchaValue> keyValuePair) => urlHelper.Action(
            "Generate",
            "DefaultCaptcha",
            new RouteValueDictionary
            {
                {"area", string.Empty},
                {captchaManager.TokenParameterName, keyValuePair.Key}
            });

        private static void SetDynamicAuthenticationCookieName()
        {
            // determine unique cookie name per application
            string cookieName = $"AUTH_{Guid.NewGuid().ToString()}";
            // This will enforce that FormsAuthentication class is loaded from configuration settings for the application.
            FormsAuthentication.Initialize();

            // The new cookie name whatever you need can go here, I needed some value from my application setting to be prefixed so I used it.
            //string newCookieName = string.Format("{0}.ASPXAUTH", ConfigurationManager.AppSettings["SomeSettingThatIsUniquetoSite"]);

            // Modifying underlying baking field that points to FormsAuthentication.FormsCookieName         
            Type type = typeof(FormsAuthentication);
            System.Reflection.FieldInfo field = type.GetField("_FormsName", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
            field.SetValue(null, cookieName);
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