namespace Fanex.ExternalAccount.Service
{
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DTO;
    using Fanex.NAP.Framework.Data;
    using System;
    using System.Collections.Generic;

    public interface IIPInformatonService
    {
        IPInformation GetFromIP(string ip);

        bool IsSpecialIP(string ip);

        IEnumerable<IPRange> GetSpecialIPs();
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

        public bool IsSpecialIP(string ip)
        {
            var ipInformation = GetFromIP(ip);

            return ipInformation != null && ipInformation.CountryShort.Equals("vn", StringComparison.CurrentCultureIgnoreCase);
        }

        public IEnumerable<IPRange> GetSpecialIPs()
            => dynamicRepository.Fetch<IPRange>(new GetAllSpecialIPCriteria());
    }
}
