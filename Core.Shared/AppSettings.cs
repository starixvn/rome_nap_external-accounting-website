namespace Fanex.ExternalAccount.Core.Shared
{
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        public static string Environment => Configs["Environment"];

        public static bool IsUAT => Environment == "UAT";

        public static string LogActionFilter_AppliedUserNames => Configs["LogActionFilter:AppliedUserNames"];

        public static string LogActionFilter_IgnoredActions => Configs["LogActionFilter:IgnoredActions"];

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;
    }
}