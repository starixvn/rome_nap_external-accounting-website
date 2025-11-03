namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.Configuration;
    using System.Web;

    public static class GoogleAnalyticHepler
    {
        private static readonly HtmlString EmptyHtml = new HtmlString(string.Empty);
        private static readonly string GoogleAnalyticId = ConfigurationManager.AppSettings["GoogleAnalyticId"];
        private static readonly string GoogleAnalyticUrl = ConfigurationManager.AppSettings["GoogleAnalyticUrl"] ?? "https://www.googletagmanager.com/gtag/js";

        public static HtmlString Render() => Render(GoogleAnalyticId);

        public static HtmlString Render(string googleAnalyticId)
        {
            if (string.IsNullOrEmpty(googleAnalyticId))
            {
                return EmptyHtml;
            }

            return new HtmlString(@"<!-- Global site tag (gtag.js) - Google Analytics -->
                                      <script async src='" + GoogleAnalyticUrl + "?id=" + GoogleAnalyticId + @"'></script>
                                          <script>
                                              window.dataLayer = window.dataLayer || [];
                                              function gtag(){dataLayer.push(arguments);}
                                              gtag('js', new Date());

                                              gtag('config', '" + GoogleAnalyticId + @"');
                                            </script>");
        }
    }
}