namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.Configuration;

    public static class GoogleAnalyticHepler
    {
        // Stub implementation for .NET Core compatibility
        private static readonly string GoogleAnalyticId = ConfigurationManager.AppSettings["GoogleAnalyticId"];
        private static readonly string GoogleAnalyticUrl = ConfigurationManager.AppSettings["GoogleAnalyticUrl"] ?? "https://www.googletagmanager.com/gtag/js";

        public static string Render() => Render(GoogleAnalyticId);

        public static string Render(string googleAnalyticId)
        {
            if (string.IsNullOrEmpty(googleAnalyticId))
            {
                return string.Empty;
            }

            return @"<!-- Global site tag (gtag.js) - Google Analytics -->
                                      <script async src='" + GoogleAnalyticUrl + "?id=" + GoogleAnalyticId + @"'></script>
                                          <script>
                                              window.dataLayer = window.dataLayer || [];
                                              function gtag(){dataLayer.push(arguments);}
                                              gtag('js', new Date());

                                              gtag('config', '" + GoogleAnalyticId + @"');
                                            </script>";
        }
    }
}