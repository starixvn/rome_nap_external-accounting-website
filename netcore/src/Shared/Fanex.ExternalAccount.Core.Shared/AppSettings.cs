namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        public static string ShowSearchAndCopyForGroups => Configs["ShowSearchAndCopyForGroups"];

        public static string CTS_Url => Configs["CTS:Url"];

        public static string Environment => Configs["Environment"];

        public static bool IsUAT => Environment == "UAT";

        public static string LogActionFilter_AppliedUserNames => Configs["LogActionFilter:AppliedUserNames"];

        public static string LogActionFilter_IgnoredActions => Configs["LogActionFilter:IgnoredActions"];

        public static string PT3DecimalSites => Configs["PT3DecimalSites"] ?? "789y";

        public static string DefaultLanguage => "en-US";

        public static Uri TelegramBotClientUri => new Uri(Configs["TelegramBotClientUri"]);

        public static string LoggingDefaultCategory => Configs["Fanex.Logging:DefaultCategory"];

        public static string RabbitMQConnectionStrings => Configs["Fanex.Logging:RabbitMQConnectionStrings"];

        public static string SentryBackendUrl => Configs["Fanex.Logging:SentryBackendUrl"];

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;

        public static bool IsOfferedCashService(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return false;
            }

            var sites = (Configs["OfferedCashServiceGroups"] ?? string.Empty);
            return sites.Contains("*") || sites.Contains(groupName);
        }
    }
}

