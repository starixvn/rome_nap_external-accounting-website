namespace Fanex.ExternalAccount.Core.Helpers
{
    using Microsoft.Extensions.Configuration;

    public static class GoogleAnalyticHepler
    {
        private static IConfiguration _configuration;

        public static void Initialize(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        // Stub implementation for .NET Core compatibility
        private static string GoogleAnalyticId => _configuration?["GoogleAnalyticId"];
        private static string GoogleAnalyticUrl => _configuration?["GoogleAnalyticUrl"] ?? "https://www.googletagmanager.com/gtag/js";

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