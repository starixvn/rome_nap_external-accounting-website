namespace Fanex.ExternalAccount.Service
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DTO;
    using Fanex.NAP.Framework.Data;
    using Utilities;

    public interface IIPInformatonService
    {
        IPInformation GetFromIP(string ip);
        bool IsSpecialIP(string ip);
        DataTable HideSpecialIP(DataTable table, bool viewSpecialIPPermission);
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

        public DataTable HideSpecialIP(DataTable table, bool viewSpecialIPPermission)
        {
            var fieldName = "lastloginip";
            if (ReferenceEquals(null, table) ||
                table.Rows.Count == 0 ||
                !table.Columns.Contains(fieldName) ||
                viewSpecialIPPermission)
            {
                return table;
            }

            var specialIPList = GetSpecialIP();
            foreach (DataRow row in table.Rows)
            {
                var ip = row.ToString(fieldName);                
                if (!string.IsNullOrWhiteSpace(ip) && specialIPList.IsExistedInList(ip))
                {
                    row[fieldName] = string.Empty;
                }
            }

            return table;
        }

        private IEnumerable<IPRange> GetSpecialIP()
            => dynamicRepository.Fetch<IPRange>(new GetAllSpecialIPCriteria());
    }
}
