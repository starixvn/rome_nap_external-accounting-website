namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System.Collections.Generic;
    using Common.Extensions;
    using Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DTO;

    public interface IIPInformatonRepository
    {
        IpRange GetIpVN();

    }
    public class IPInformatonRepository : IIPInformatonRepository
    {
        public IpRange GetIpVN()
        {
            var ipv4List = new List<Ipv4Range>();
            var ipv6List = new List<Ipv6Range>();
            var criteria = new GetAllSpecialIPCriteria();

            using (var db = new ObjectDb(criteria.GetSettingKey()))
            {
                using (var reader = db.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        ipv4List.Add(new Ipv4Range
                        {
                            IPFrom = reader.ToLong("IPFrom"),
                            IPTo = reader.ToLong("IPTo")
                        });
                    }

                    reader.NextResult();
                    while (reader.Read())
                    {
                        ipv6List.Add(new Ipv6Range
                        {
                            IPFrom = reader.GetOrdinal("IPFrom").ToString(),
                            IPTo = reader.GetOrdinal("IPTo").ToString()
                        });
                    }
                }
            }

            return new IpRange(ipv4List, ipv6List);
        }
    }
}