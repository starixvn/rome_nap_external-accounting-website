namespace Fanex.ExternalAccount.Core.Shared
{
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

        internal static NameValueCollection Configs { get; set; } = ConfigurationManager.AppSettings;
    }
}