namespace Fanex.ExternalAccount.Core.Helpers
{
    using System.Web;
    using Context;
    using DTO;
    using UAParser;

    public static class RequestHelper
    {
        public static Request GetRequestInfo(string browserSize = null)
        {
            var userAgent = HttpContext.Current.Request.UserAgent ?? string.Empty;
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