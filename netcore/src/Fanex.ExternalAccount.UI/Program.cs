namespace Fanex.ExternalAccount
{
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using Core.Authorization.ProductStatus;
    using Core.Filters;
    using Fanex.Caching;
    using Fanex.Customer.Client;
    using Fanex.Customer.Client.Configurations;
    using Fanex.Customer.Client.NETFramework;
    using Fanex.Data;
    using Fanex.ExternalAccount.Core.Configurations;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.UI.Helpers;
    using Fanex.Logging;
    using Fanex.Logging.Extensions;
    using Fanex.Logging.Extensions.Database;
    using Fanex.Logging.RabbitMQ;
    using Fanex.Logging.Sentry.RavenClient;
    using Fanex.ProductStatus.Setting.Configuration;
    using Logging.Extensions.Web;
    using ONEbook.UM;
    using ONEbook.UM.AspNetMVC.Filters;
    using ONEbook.UM.Configurations;
    using ONEbook.UM.Helpers;
    using ONEbook.UM.NETFramework;
    using ONEbook.UM.Services;
    using ONEbook.UM.Transport;
    using SharpRaven;
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Linq;
    using Microsoft.AspNetCore.Builder;
    using Microsoft.AspNetCore.Hosting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Microsoft.Extensions.DependencyInjection;
    using Microsoft.Extensions.Hosting;
    using Microsoft.AspNetCore.Mvc.Filters;
    using Microsoft.AspNetCore.Routing;

    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Configuration
            builder.Configuration
                .SetBasePath(builder.Environment.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true)
                .AddXmlFile("Web.config", optional: true, reloadOnChange: true)
                .AddEnvironmentVariables();

            ConfigureServices(builder.Services, builder.Configuration);
            
            var app = builder.Build();
            
            ConfigureApplication(app);
            
            app.Run();
        }

        private static void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            // Add MVC services with areas support
            services.AddControllersWithViews(options =>
            {
                // Register global filters
                options.Filters.Add(GetCheckUMAttribute());
                options.Filters.Add(new CheckBelongingFilter());
                options.Filters.Add(new LogActionFilterAttribute());
                options.Filters.Add(new SecurityCodeFilterAttribute());
            });
            
            // Add session
            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromMinutes(20);
                options.Cookie.Name = ".EXTNAPFORMSAUTH";
            });

            // Initialize logging
            var logEngines = new List<ILogEngine>();
            var rabbitMQConnectionString = configuration.GetValue<string>("RabbitMQConnectionStrings");
            if (!string.IsNullOrWhiteSpace(rabbitMQConnectionString))
            {
                logEngines.Add(new RabbitMQLogging(rabbitMQConnectionString));
            }
            var sentryBackendUrl = configuration.GetValue<string>("SentryBackendUrl");
            if (!string.IsNullOrWhiteSpace(sentryBackendUrl))
            {
                logEngines.Add(new SentryLogging(new SentryEngineOptions
                {
                    Dsn = new Dsn(sentryBackendUrl)
                }));
            }
            
            var loggingDefaultCategory = configuration.GetValue<string>("LoggingDefaultCategory") ?? "Default";
            LogManager
                .SetDefaultLogCategory(loggingDefaultCategory)
                .Use(logEngines);

            ExceptionHandler
                .UseConfig(new LoggingConfiguration()
                .IgnoreException<DivideByZeroException>()
                .HideSensitiveInfo(GetRegexToHideSensitiveInfo(new List<string> { "pass", "pwd", "security", "Password" }))
                .GetHttpHeaderInfo()
                .GetHttpSessionInfo())
                .UseGetDbExceptionInfo(DatabaseInfoGetters.GetDbExceptionInfo);

            // Initialize Customer Client
            CustomerClientManager.UseConfig(new CustomerClientConfiguration().Initialize());

            // Initialize DbSettingProvider
            DbSettingProviderManager
                 .StartNewSession()
                 .Register(true)
                 .Run();

            // Initialize StaticResources
            StaticResources.Init();

            // Configure Captcha
            var defaultCaptchaManager = (DefaultCaptchaManager)CaptchaUtils.CaptchaManager;
            defaultCaptchaManager.ImageUrlFactory =
                (helper, pair) => ImageUrlFactory(defaultCaptchaManager, helper, pair);
            defaultCaptchaManager.RefreshUrlFactory = RefreshUrlFactory;

            // Configure ProductStatus
            var productStatusConfig = new ProductStatusConfiguration()
                .UseAuthorizeService(new ProductStatusAuthorizeService())
                .UseProductStatusActor(new ProductStatusActor())
                .SetUnAuthorizedUrl("~/Customer/AccessDenied")
                .UseCustomerProductList(MultipleEditProductStatus.GetProductList);
            ProductStatusConfigurationHandler.UseConfig(productStatusConfig);

            // Initialize RemotingRef
            var remotingRefService = configuration.GetValue<string>("Fanex.RemotingRef.Service");
            if (!string.IsNullOrWhiteSpace(remotingRefService))
            {
                Fanex.RemotingRef.RefDataTypeContainer
                    .ConnectTo(new Uri(remotingRefService))
                    .FromApplication("Ext NAP")
                    .SupportOnlyDefaultLanguage();
            }

            // Register Redis Caching
            RedisCachingRegister.Register();

            // Configure UM
            var umConfig = new UMClientConfiguration().Initialize();
            UMClientManager
               .UseConfig(umConfig)
               .UseCacheService(new UMCacheService(new CacheService()))
               .UseRestClient(new UMRestClient(umConfig))
               .UseHttpContext(new HttpContextHelper());
        }

        private static void ConfigureApplication(WebApplication app)
        {
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Shared/Error");
            }

            app.UseStaticFiles();
            app.UseRouting();
            app.UseSession();

            app.MapControllerRoute(
                name: "areas",
                pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "Default4",
                pattern: "{controller}/{action}/{id}/{custId}/{uplineId}/{username}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = "", custId = "", uplineId = "", username = "", groupname = "" });

            app.MapControllerRoute(
                name: "Default3",
                pattern: "{controller}/{action}/{id}/{username}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = "", username = "", groupname = "" });

            app.MapControllerRoute(
                name: "Default2",
                pattern: "{controller}/{action}/{id}/{groupname}",
                defaults: new { controller = "Home", action = "Index", id = "", groupname = "" });

            app.MapControllerRoute(
                name: "Default",
                pattern: "{controller}/{action}/{id}",
                defaults: new { controller = "Home", action = "Index", id = "" });
        }

        private static void RegisterGlobalFilters(FilterCollection filters)
        {
            filters.Add(GetCheckUMAttribute());
            filters.Add(new CheckBelongingFilter());
            filters.Add(new LogActionFilterAttribute());
            filters.Add(new SecurityCodeFilterAttribute());
        }

        // TODO: Migrate Captcha URL factories to ASP.NET Core IUrlHelper
        // These methods need to be refactored to use ASP.NET Core's IUrlHelper
        // For now, keeping the structure but will need HttpContext injection
        private static string RefreshUrlFactory(object urlHelper, KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            // TODO: Implement using ASP.NET Core IUrlHelper
            return "/DefaultCaptcha/Refresh";
        }

        private static string ImageUrlFactory(DefaultCaptchaManager captchaManager, object urlHelper,
            KeyValuePair<string, ICaptchaValue> keyValuePair)
        {
            // TODO: Implement using ASP.NET Core IUrlHelper
            return $"/DefaultCaptcha/Generate?{captchaManager.TokenParameterName}={keyValuePair.Key}";
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

        private static List<string> GetRegexToHideSensitiveInfo(IEnumerable<string> sensitiveInfo)
        {
            var result = new List<string>();
            sensitiveInfo?.ToList().ForEach(info =>
            {
                result.Add($"(?:{info}=)(?<paramValue>[^&|^\\s]+)");
                result.Add($"(\"{info}\":)(?<paramValue>[^,|^\r]+)");
            });

            return result;
        }
    }
}

