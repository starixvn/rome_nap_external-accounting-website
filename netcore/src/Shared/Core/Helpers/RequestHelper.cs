namespace Fanex.ExternalAccount.Core.Helpers
{
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Context;
    using DTO;
    using UAParser;

    public static class RequestHelper
    {
        private static IHttpContextAccessor? _httpContextAccessor;

        public static void SetHttpContextAccessor(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        private static HttpContext? HttpContext
        {
            get
            {
                if (_httpContextAccessor?.HttpContext != null)
                {
                    return _httpContextAccessor.HttpContext;
                }
                
                // Fallback
                try
                {
                    var serviceProvider = new ServiceCollection().BuildServiceProvider();
                    var accessor = serviceProvider.GetService<IHttpContextAccessor>();
                    return accessor?.HttpContext;
                }
                catch
                {
                    return null;
                }
            }
        }

        public static Request GetRequestInfo(string browserSize = null)
        {
            var request = HttpContext?.Request;
            var userAgent = request?.Headers["User-Agent"].ToString() ?? string.Empty;
            var userAgentParser = Parser.GetDefault();
            var clientInfo = userAgentParser.Parse(userAgent);
            var clientUserAgent = clientInfo.UserAgent;

            var browser = $"{clientUserAgent.Family} {clientUserAgent.Major}.{clientUserAgent.Minor}.{clientUserAgent.Patch ?? "0"}";
            var operateSystem = $"{clientInfo.OS.Family} {clientInfo.OS.Major ?? "0"}.{clientInfo.OS.Minor ?? "0"}.{clientInfo.OS.Patch ?? "0"}";
            var device = $"{clientInfo.Device.Family}, IsCrawler: {clientInfo.Device.IsSpider}";

            if(!string.IsNullOrWhiteSpace(browserSize))
            {
                browser += $", {browserSize}";
            }

            return new Request
            {
                Browser = browser,
                CountryCode = CurrentContext.Instance.CountryCode,
                DomainName = CurrentContext.Instance.DomainName,
                IP = CurrentContext.Instance.Ip4Address,
                NetSessionId = CurrentSession.Instance.NetSessionId,
                Platform = $"{operateSystem}, Device: {device}"
            };
        }
    }
}
