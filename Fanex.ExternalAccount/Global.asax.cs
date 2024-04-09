namespace Fanex.ExternalAccount
{
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Core.Authorization.ProductStatus;
    using Core.Filters;
    using CustomerSetting.Client40;
    using Data.Configuration;
    using Fanex.Caching;
    using Fanex.ExternalAccount.UI.Helpers;
    using Fanex.Logging.RabbitMQ;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.Logging;
    using Logging.Extensions.Web;
    using ONEbook.UM;
    using ONEbook.UM.AspNetMVC.Filters;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.NETFramework;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using ProductStatus.Setting.Configuration;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using Fanex.ExternalAccount.Core.Shared;
    using SharpRaven;
    using Fanex.Logging.Extensions;
    using System.Linq;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Customer.Client;
    using Fanex.Customer.Client.Configurations;
    using Fanex.Customer.Client.NETFramework;
    using Fanex.ExternalAccount.Core.Configurations;

    public class MvcApplication : HttpApplication
    {
        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(GetCheckUMAttribute());
            filters.Add(new CheckBelongingFilter());
            filters.Add(new LogActionFilterAttribute());
            filters.Add(new SecurityCodeFilterAttribute());
        }

        public static void RegisterRoutes(RouteCollection routes)
        {
            routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
            routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*\.aspx(/.*)?" });
            routes.IgnoreRoute("{*allashx}", new { allashx = @".*\.ashx(/.*)?" });
            routes.IgnoreRoute("{*favicon}", new { favicon = "(.*/)?favicon.ico(/.*)?" });
            routes.IgnoreRoute("base/{*pathInfo}");
            routes.MapRoute(
                "Default",
                "{controller}/{action}/{id}",
#pragma warning disable S1192 // String literals should not be duplicated
                new { controller = "Home", action = "Index", id = UrlParameter.Optional });
#pragma warning restore S1192 // String literals should not be duplicated

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
            var logEngines = new List<ILogEngine>();
            if (!string.IsNullOrWhiteSpace(AppSettings.RabbitMQConnectionStrings))
            {
                logEngines.Add(new RabbitMQLogging(AppSettings.RabbitMQConnectionStrings));
            }
            if (!string.IsNullOrWhiteSpace(AppSettings.SentryBackendUrl))
            {
                logEngines.Add(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(AppSettings.SentryBackendUrl)
                }));
            }
            LogManager
                .SetDefaultLogCategory(AppSettings.LoggingDefaultCategory)
                .Use(logEngines);

            ExceptionHandler
                .UseConfig(new LoggingConfiguration()
                .IgnoreException<DivideByZeroException>()
                .HideSensitiveInfo(new List<string> { "pass", "pwd", "security", "Password" })
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            AreaRegistration.RegisterAllAreas();
            CustomerSettingRegistration.Init();
            CustomerClientManager.UseConfig(new CustomerClientConfiguration().Initialize());

            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DbSettingProviderManager.Current.Start();
            StaticResources.Init();

            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory =
                (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;

            MvcHandler.DisableMvcResponseHeader = true;

            var productStatusConfig = new ProductStatusConfiguration()
                .UseAuthorizeService(new ProductStatusAuthorizeService())
                .UseProductStatusActor(new ProductStatusActor())
                .SetUnAuthorizedUrl("~/Customer/AccessDenied")
                .UseCustomerProductList(MultipleEditProductStatus.GetProductList);
            ProductStatusConfigurationHandler.UseConfig(productStatusConfig);
            Fanex.RemotingRef.RefDataTypeContainer
                .ConnectTo(new Uri(ConfigurationManager.AppSettings["Fanex.RemotingRef.Service"]))
                .FromApplication("Ext NAP")
                .SupportOnlyDefaultLanguage();

            RedisCachingRegister.Register();
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }

        protected void Application_BeginRequest()
        {
            Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);

            AddCORSToCheckNetworkSpeed();
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

        private void AddCORSToCheckNetworkSpeed()
        {
            const string SampleTestNetworkSpeedFileName = "sample_test_network_speed_1.jpg";
            var fileName = VirtualPathUtility.GetFileName(Context.Request.FilePath);

            if (fileName.Equals(SampleTestNetworkSpeedFileName, StringComparison.OrdinalIgnoreCase))
            {
                Context.Response.AddHeader("Access-Control-Allow-Origin", "*");
            }
        }

        private static CheckUMFilterAttribute GetCheckUMAttribute()
        {
            var umConfig = new UMClientConfiguration().Initialize();

            UMClientManager
               .UseConfig(umConfig)
               .UseCacheService(new UMCacheService(new CacheService()))
               .UseRestClient(new UMRestClient(umConfig))
               .UseHttpContext(new HttpContextHelper());

            var maintenanceService = new MaintenanceService(
                      UMClientManager.RestClient,
                      UMClientManager.CacheService,
                      UMClientManager.UMClientConfigs,
                      UMClientManager.HttpContext);

            return new CheckUMFilterAttribute(maintenanceService, umConfig);
        }
    }
}