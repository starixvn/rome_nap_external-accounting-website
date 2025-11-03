using System.Globalization;
using System.Numerics;

namespace Fanex.ExternalAccount.DTO
{
    public class Ipv6Range
    {
        public string IPFrom { get; set; }

        public string IPTo { get; set; }

        public BigInteger FromNumber
            => BigInteger.Parse(IPFrom, NumberStyles.AllowHexSpecifier);

        public BigInteger ToNumber
            => BigInteger.Parse(IPTo, NumberStyles.AllowHexSpecifier);

    }
}
