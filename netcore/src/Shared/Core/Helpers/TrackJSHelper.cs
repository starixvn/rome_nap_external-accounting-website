namespace Fanex.ExternalAccount.Core.Helpers
{
    using Microsoft.Extensions.Configuration;
    using Context;

    public static class TrackJSHelper
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Stub implementation for .NET Core compatibility
        private static string TrackJSToken => _configuration?["TrackJSToken"];
        private static string TrackJSUrl => _configuration?["TrackJSUrl"] ?? "https://cdn.trackjs.com/releases/current/tracker.js";

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