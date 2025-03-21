namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.IPInformation;
    using Fanex.ExternalAccount.DTO;

    public interface IIPInformatonRepository
    {
        IpRange GetIpVN();

    }
    public class IPInformatonRepository : IIPInformatonRepository
    {
        private readonly IDynamicRepository _dynamicRepository;

        public IPInformatonRepository() : this(null)
        { }

        public IPInformatonRepository(IDynamicRepository dynamicRepository)
        {
            this._dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IpRange GetIpVN()
        {
            var criteria = new GetAllSpecialIPCriteria();
            var result = _dynamicRepository.FetchMultiple<Ipv4Range,Ipv6Range>(criteria);
            return new IpRange(result.Item1, result.Item2);
        }
    }
}