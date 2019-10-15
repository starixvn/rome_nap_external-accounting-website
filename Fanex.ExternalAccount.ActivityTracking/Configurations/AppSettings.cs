namespace Fanex.ExternalAccount.ActivityTracking.Configurations
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        [ThreadStatic]
        private static NameValueCollection appSettingValues;

        public static NameValueCollection AppSettingValues
        {
            get => appSettingValues ?? ConfigurationManager.AppSettings;
            set => appSettingValues = value;
        }

        public static string LoggingDefaultCategory => AppSettingValues["Fanex.Logging:DefaultCategory"];

        public static string LogLibs => AppSettingValues["LogLibs"];

        public static string RabbitMQConnectionStrings => AppSettingValues["Fanex.Logging:RabbitMQConnectionStrings"];

        public static string SentryBackendUrl => AppSettingValues["Fanex.Logging:SentryBackendUrl"];

        public static string SentryUrl => AppSettingValues["Fanex.Logging:SentryUrl"];

        public static string RavenLibPath => AppSettingValues["RavenLibPath"];
    }
}
