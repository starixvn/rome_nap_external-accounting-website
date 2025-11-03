namespace Fanex.ProductStatus.Setting.Configuration
{
    using System;
    using System.Collections.Generic;
    using Services;

    public static class ProductStatusConfigurationHandler
    {
        private static ProductStatusConfiguration statusSettings;

        public static void UseConfig(ProductStatusConfiguration settings)
        {
            statusSettings = settings;

            Verify();
        }

        internal static IProductStatusAuthorizeService AuthorizeService
            => statusSettings.AuthorizeService;

        internal static IProductStatusActor Actor
            => statusSettings.Actor;

        internal static string UnAuthorizedUrl
            => statusSettings.UnAuthorizedUrl;

        internal static Func<IEnumerable<Product>> CustomerProductListGetter
            => statusSettings.CustomerProductListGetter;

        private static void Verify()
        {
            RequireProductStatusSettings();

            RequireAuthorizeService();

            RequireActor();

            RequireUnAuthorizedUrl();
        }

        private static void RequireProductStatusSettings()
        {
            if (statusSettings == null)
            {
                throw new ArgumentNullException(
                    $"ProductStatus {nameof(statusSettings)}",
                    "Value cannot be null.");
            }
        }

        private static void RequireAuthorizeService()
        {
            if (statusSettings.AuthorizeService == null)
            {
                throw new ArgumentNullException(
                    $"ProductStatus {nameof(statusSettings.AuthorizeService)}",
                    "Value cannot be null.");
            }
        }

        private static void RequireActor()
        {
            if (statusSettings.Actor == null)
            {
                throw new ArgumentNullException(
                    $"ProductStatus {nameof(statusSettings.Actor)}",
                    "Value cannot be null.");
            }
        }

        private static void RequireUnAuthorizedUrl()
        {
            if (string.IsNullOrWhiteSpace(statusSettings.UnAuthorizedUrl))
            {
                throw new ArgumentNullException(
                    $"ProductStatus {nameof(statusSettings.UnAuthorizedUrl)}",
                    "Value cannot be null or empty.");
            }
        }
    }
}