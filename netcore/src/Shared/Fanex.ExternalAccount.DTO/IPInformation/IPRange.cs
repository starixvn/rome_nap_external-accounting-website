using System.Collections.Generic;

namespace Fanex.ExternalAccount.DTO
{
    public class IpRange
    {
        public IpRange(IEnumerable<Ipv4Range> ipV4List, IEnumerable<Ipv6Range> ipV6List)
        {
            Ipv4List = ipV4List;
            Ipv6List = ipV6List;
        }

        public IEnumerable<Ipv4Range> Ipv4List { get; set; }

        public IEnumerable<Ipv6Range> Ipv6List { get; set; }
    }
}
