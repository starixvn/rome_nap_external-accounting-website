namespace Fanex.ExternalAccount.Core.Context
{
    using Service;
    using System.Globalization;
    using System.Web;

    public class CurrentContext : ICurrentContext
    {
        private readonly IIPInformatonService ipInformationService = new IPInformationService();
        public static ICurrentContext Instance => new CurrentContext();

        public HttpContext Context => HttpContext.Current;

        public string Ip4Address
        {
            get
            {
                var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"] ?? HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                return ip.Split(',')[0];
            }
        }

        public string Ip6Address
        {
            get
            {
                var ip = HttpContext.Current.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

                if (string.IsNullOrWhiteSpace(ip))
                {
                    ip = HttpContext.Current.Request.ServerVariables["REMOTE_ADDR"];
                }

                return ip;
            }
        }

        public string DomainName => HttpContext.Current.Request.ServerVariables["SERVER_NAME"];

        public string FullDomainName
        {
            get
            {
                var variables = HttpContext.Current.Request.ServerVariables;

                var protocol =
                    variables["SERVER_PORT_SECURE"] == "1" ? "https://" : "http://"; //DevSkim: ignore DS137138
                var domain = variables["SERVER_NAME"];
                var port = variables["SERVER_PORT"];

                return protocol + domain + ":" + port;
            }
        }

        public string CountryCode
        {
            get
            {
                var regionInfo = ResolveCountry();

                return regionInfo?.EnglishName;
            }
        }

        private RegionInfo ResolveCountry()
        {
            try
            {
                return new RegionInfo(ipInformationService.GetFromIP(Ip4Address).CountryShort);
            }
            catch
            {
                return null;
            }
        }
    }
}