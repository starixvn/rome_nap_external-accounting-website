namespace Fanex.ExternalAccount
{
    using System;
    using System.Collections.Generic;
    using System.Web;
    using System.Web.Mvc;
    using System.Web.Routing;
    using CaptchaMvc.Infrastructure;
    using CaptchaMvc.Interface;
    using CasinoLibrary.SBAgent;
    using Core.Authorization.ProductStatus;
    using Core.Context;
    using Core.Filters;
    using Core.Shared;
    using CustomerSetting.Client40;
    using Data.Configuration;
    using Fanex.ExternalAccount.UI.Helpers;
    using GoldDeluxe.CoreSetting.Mappings;
    using Logging.Extensions.Database;
    using Logging.Extensions.Web;
    using Module.LeftPanel.ViewModels;
    using ONEbook.UM.Filters;
    using ProductStatus.Setting.Configuration;
    using Sunplus.Libraries.APILogger;

    public class MvcApplication : HttpApplication
    {
        private static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new CheckUMFilter());
            filters.Add(new CheckBelongingFilter());
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
            AreaRegistration.RegisterAllAreas();
            CustomerSettingRegistration.Init();
                        
            RegisterGlobalFilters(GlobalFilters.Filters);
            RegisterRoutes(RouteTable.Routes);
            DbSettingProviderManager.Current.Start();
            AutoMapperConfiguration.Configure();
            StaticResources.Init();

            var logger = new CasinoAPILogger();
            var manager = new APILogManager(logger);
            const string pattern = "AutoInsert*|Update*|Insert*|Move*|Reset*|FullSettlementResetMaxTransfer|Rollback*";
            manager.Register(typeof(Customer), pattern);
            manager.Register(typeof(DownlineList), pattern);

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

            var productStatusConfig = new ProductStatusConfiguration()
                .UseAuthorizeService(new ProductStatusAuthorizeService())
                .UseProductStatusActor(new ProductStatusActor())
                .SetUnAuthorizedUrl("~/Customer/AccessDenied")
                .UseCustomerProductList(MultipleEditProductStatus.GetCustomerProductList);
            ProductStatusConfigurationHandler.UseConfig(productStatusConfig);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            ExceptionHandler.Handle(Server.GetLastError());
        }

        protected void Application_BeginRequest()
        {
            Context.Response.Cache.SetCacheability(HttpCacheability.NoCache);
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