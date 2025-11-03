namespace Fanex.ExternalAccount.Core.Context
{
    using System.Web;

    public interface ICurrentContext
    {
        HttpContext Context { get; }

        string Ip4Address { get; }

        string Ip6Address { get; }

        string DomainName { get; }

        string FullDomainName { get; }

        string CountryCode { get; }
    }
}
