namespace Fanex.ExternalAccount.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Sockets;
    using System.Numerics;
    using Fanex.Caching;
    using Fanex.Caching.Memory;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DTO;

    public interface IIPInformatonService
    {
        IPInformation GetFromIP(string ip);

        bool IsSpecialIP(string ipAddress);
    }

    public class IPInformationService : IIPInformatonService
    {
        private readonly IDynamicRepository dynamicRepository;
        private readonly IIPInformatonRepository ipInformationRepository;
        private readonly ICacheService Cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        public IPInformationService() : this(null, null)
        {

        }

        public IPInformationService(IDynamicRepository dynamicRepository, IIPInformatonRepository ipInformationRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.ipInformationRepository = ipInformationRepository ?? new IPInformatonRepository();
        }

        public IPInformation GetFromIP(string ip)
            => dynamicRepository.Get<IPInformation>(new GetIPInformationCriteria { IP = ip });

        public bool IsSpecialIP(string ipAddress)
        {
            var ipVnRange = GetSpecialIPs();

            var isIpv6 = IsIPV6(ipAddress);
            if (isIpv6)
            {
                var ipv6Number = ConvertIpv6ToNumber(ipAddress);
                return ipVnRange.Ipv6List.Any(ip => ip.FromNumber <= ipv6Number && ipv6Number <= ip.ToNumber);
            }

            double ipNumber = ConvertIPToNumber(ipAddress);
            return ipVnRange.Ipv4List.Any(ip => ip.IPFrom <= ipNumber && ipNumber <= ip.IPTo);
        }

        private IpRange GetSpecialIPs()
        {
            const string key = "Fanex.NAP.External.Site.IPVN";
            Cache.TryGet<IpRange>(key, out var result);

            if (result == null)
            {
                result = ipInformationRepository.GetIpVN();
                Cache.Set(key, result);
            }

            return result;
        }

        private static bool IsIPV6(string clientIP)
            => IPAddress.TryParse(clientIP, out IPAddress address)
                && address.AddressFamily == AddressFamily.InterNetworkV6;

        public static BigInteger ConvertIpv6ToNumber(string ipv6)
        {
            BigInteger bigInteger = 0;
            if (IPAddress.TryParse(ipv6, out IPAddress address))
            {
                byte[] numArray = address.GetAddressBytes();
                if (BitConverter.IsLittleEndian)
                {
                    List<byte> byteList = new List<byte>(numArray);
                    byteList.Reverse();
                    numArray = byteList.ToArray();
                }
                bigInteger = numArray.Length <= 8 ? BitConverter.ToUInt32(numArray, 0) : ((BigInteger)BitConverter.ToUInt64(numArray, 8) << 64) + BitConverter.ToUInt64(numArray, 0);
            }

            return bigInteger;
        }

        private static long ConvertIPToNumber(string ipAddress)
        {
            if (IPAddress.TryParse(ipAddress, out IPAddress parsedIpAddress))
            {
                var addressBytes = parsedIpAddress.GetAddressBytes();
                Array.Reverse(addressBytes);

                return BitConverter.ToUInt32(addressBytes, 0);
            }

            return 0;
        }
    }
}
