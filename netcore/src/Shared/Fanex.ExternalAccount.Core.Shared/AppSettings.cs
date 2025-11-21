namespace Fanex.ExternalAccount.Core.Shared
{
    using System;
    using Microsoft.Extensions.Configuration;

    public static class AppSettings
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public static string ShowSearchAndCopyForGroups => _configuration?["ShowSearchAndCopyForGroups"];

        public static string CTS_Url => _configuration?["CTS:Url"];

        public static string Environment => _configuration?["Environment"];

        public static bool IsUAT => Environment == "UAT";

        public static string LogActionFilter_AppliedUserNames => _configuration?["LogActionFilter:AppliedUserNames"];

        public static string LogActionFilter_IgnoredActions => _configuration?["LogActionFilter:IgnoredActions"];

        public static string PT3DecimalSites => _configuration?["PT3DecimalSites"] ?? "789y";

        public static string DefaultLanguage => "en-US";

        public static Uri TelegramBotClientUri => _configuration != null ? new Uri(_configuration["TelegramBotClientUri"]) : null;

        public static string LoggingDefaultCategory => _configuration?["Fanex.Logging:DefaultCategory"];

        public static string RabbitMQConnectionStrings => _configuration?["Fanex.Logging:RabbitMQConnectionStrings"];

        public static string SentryBackendUrl => _configuration?["Fanex.Logging:SentryBackendUrl"];

        public static bool IsOfferedCashService(string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                return false;
            }

            var sites = (_configuration?["OfferedCashServiceGroups"] ?? string.Empty);
            return sites.Contains("*") || sites.Contains(groupName);
        }
    }
}

