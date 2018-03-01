namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.Configuration;
    using System.Web;
    using Context;

    public static class TrackJSHelper
    {
        private static readonly HtmlString EmptyHtml = new HtmlString(string.Empty);
        private static readonly string TrackJSToken = ConfigurationManager.AppSettings["TrackJSToken"];
        private static readonly string TrackJSUrl = ConfigurationManager.AppSettings["TrackJSUrl"] ?? "https://cdn.trackjs.com/releases/current/tracker.js";

        public static HtmlString Render() => Render(TrackJSToken);

        public static HtmlString Render(string trackJSToken)
        {
            if (string.IsNullOrEmpty(trackJSToken))
            {
                return EmptyHtml;
            }

            return new HtmlString(@"<!-- BEGIN TRACKJS -->
                                    <script type='text/javascript'>window._trackJs = { token: '"
                                    + TrackJSToken
                                    + "', application: 'EXT-NAP', userId: '"
                                    + CurrentSession.Instance.UserName + @"' };</script>
                                    <script type='text/javascript' src='" + TrackJSUrl + @"'></script>
                                <!-- END TRACKJS -->");
        }
    }
}