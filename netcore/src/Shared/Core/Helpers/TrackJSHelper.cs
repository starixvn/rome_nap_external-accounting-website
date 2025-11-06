namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.Configuration;
    using Context;

    public static class TrackJSHelper
    {
        // Stub implementation for .NET Core compatibility
        private static readonly string TrackJSToken = ConfigurationManager.AppSettings["TrackJSToken"];
        private static readonly string TrackJSUrl = ConfigurationManager.AppSettings["TrackJSUrl"] ?? "https://cdn.trackjs.com/releases/current/tracker.js";

        public static string Render() => Render(TrackJSToken);

        public static string Render(string trackJSToken)
        {
            if (string.IsNullOrEmpty(trackJSToken))
            {
                return string.Empty;
            }

            return @"<!-- BEGIN TRACKJS -->
                                    <script type='text/javascript'>window._trackJs = { token: '"
                                    + TrackJSToken
                                    + "', application: 'EXT-NAP', userId: '"
                                    + CurrentSession.Instance.UserName + @"' };</script>
                                    <script type='text/javascript' src='" + TrackJSUrl + @"'></script>
                                <!-- END TRACKJS -->";
        }
    }
}