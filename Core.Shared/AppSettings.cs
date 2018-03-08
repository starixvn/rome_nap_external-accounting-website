namespace Fanex.ExternalAccount.Core.Shared
{
    using System.Collections.Specialized;
    using System.Configuration;

    public static class AppSettings
    {
        public static string Environment => Configs["Environment"];

        public static bool IsUAT => Environment == "UAT";

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;
    }
}