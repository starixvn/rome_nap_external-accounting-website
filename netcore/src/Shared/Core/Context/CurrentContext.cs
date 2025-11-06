namespace Fanex.ExternalAccount.Core.Context
{
    using System.Globalization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.Extensions.DependencyInjection;
    using Fanex.ExternalAccount.Service;

    public class CurrentContext : ICurrentContext
    {
        private readonly IIPInformatonService ipInformationService = new IPInformationService();
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
                
                // Fallback: try to get from service locator if available
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

        public static ICurrentContext Instance => new CurrentContext();

        public Microsoft.AspNetCore.Http.HttpContext Context => HttpContext!;

        public string Ip4Address
        {
            get
            {
                if (HttpContext?.Request == null)
                    return string.Empty;

                // In ASP.NET Core, we get IP from headers, not ServerVariables
                var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    return forwardedFor.Split(',')[0].Trim();
                }

                var remoteIp = HttpContext.Connection.RemoteIpAddress?.ToString();
                return remoteIp ?? string.Empty;
            }
        }

        public string Ip6Address
        {
            get
            {
                if (HttpContext?.Request == null)
                    return string.Empty;

                var forwardedFor = HttpContext.Request.Headers["X-Forwarded-For"].ToString();
                if (!string.IsNullOrWhiteSpace(forwardedFor))
                {
                    return forwardedFor;
                }

                return HttpContext.Connection.RemoteIpAddress?.ToString() ?? string.Empty;
            }
        }

        public string DomainName
        {
            get
            {
                if (HttpContext?.Request == null)
                    return string.Empty;

                return HttpContext.Request.Host.Host;
            }
        }

        public string FullDomainName
        {
            get
            {
                if (HttpContext?.Request == null)
                    return string.Empty;

                var scheme = HttpContext.Request.Scheme;
                var host = HttpContext.Request.Host.Host;
                var port = HttpContext.Request.Host.Port;

                if (port.HasValue && ((scheme == "http" && port != 80) || (scheme == "https" && port != 443)))
                {
                    return $"{scheme}://{host}:{port.Value}";
                }

                return $"{scheme}://{host}";
            }
        }

        public string CountryCode
        {
            get
            {
                var regionInfo = ResolveCountry();

                return regionInfo?.EnglishName ?? string.Empty;
            }
        }

        private RegionInfo? ResolveCountry()
        {
            try
            {
                var ipInfo = ipInformationService.GetFromIP(Ip4Address);
                if (ipInfo == null || string.IsNullOrWhiteSpace(ipInfo.CountryShort))
                {
                    return null;
                }
                return new RegionInfo(ipInfo.CountryShort);
            }
            catch
            {
                return null;
            }
        }
    }
}
