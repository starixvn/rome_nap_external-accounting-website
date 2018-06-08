namespace Fanex.ExternalAccount.Service
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text.RegularExpressions;
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DTO;
    using Fanex.NAP.Framework.Data;

    public interface IIPInformatonService
    {
        IPInformation GetFromIP(string ip);
        bool Is789IP(string ip);
        bool IsExistedInList(string ip, IEnumerable<IPRange> ipList);
        IEnumerable<IPRange> Get789IP();
    }

    public class IPInformationService : IIPInformatonService
    {
        private readonly IDynamicRepository dynamicRepository;

        public IPInformationService() : this(null)
        {

        }

        public IPInformationService(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IPInformation GetFromIP(string ip)
            => dynamicRepository.Get<IPInformation>(new GetIPInformationCriteria { IP = ip });

        public bool Is789IP(string ip)
        {
            var ipInformation = GetFromIP(ip);

            return ipInformation != null && ipInformation.CountryShort.Equals("vn", StringComparison.CurrentCultureIgnoreCase);
        }

        public IEnumerable<IPRange> Get789IP()
            => dynamicRepository.Fetch<IPRange>(new GetAll789IPCriteria());

        public bool IsExistedInList(string ip, IEnumerable<IPRange> ipList)
        {
            var ipNumber = ConvertIpToNumber(ip);

            return ipList.Any(i => i.IPFrom <= ipNumber && i.IPTo >= ipNumber);
        }        

        private static double ConvertIpToNumber(string ip)
        {
            var num = 0.0;
            if (IsValidIp(ip))
            {
                var strArray = ip.Split('.');
                num = (((double.Parse(strArray[0]) * 16777216.0) + (double.Parse(strArray[1]) * 65536.0)) +
                    (double.Parse(strArray[2]) * 256.0)) + double.Parse(strArray[3]);
            }

            return num;
        }

        private static bool IsValidIp(string ip)
        {
            if (string.IsNullOrEmpty(ip))
            {
                return false;
            }

            var regex = new Regex(@"(?<First>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Second>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Third>2[0-4]\d|25[0-5]|[01]?\d\d?)\.(?<Fourth>2[0-4]\d|25[0-5]|[01]?\d\d?)");
            return regex.IsMatch(ip, 0);
        }
    }
}
