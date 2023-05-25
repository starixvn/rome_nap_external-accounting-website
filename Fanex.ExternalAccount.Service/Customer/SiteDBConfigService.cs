using Fanex.Caching;
using Fanex.ExternalAccount.DataAccess.Customer;
using Fanex.ExternalAccount.DTO;
using Fanex.NAP.Framework.Data;
using System;
using System.Linq;

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    public interface ISiteDBConfigService
    {
        SiteDB GetSiteDBConfig(int custId);
    }

    public class SiteDBConfigService : ISiteDBConfigService
    {
        private readonly ICacheService cacheService;
        private readonly IDynamicRepository repository;

        public SiteDBConfigService() : this(null, null)
        {

        }
        public SiteDBConfigService(ICacheService cacheService, IDynamicRepository repository)
        {
            this.cacheService = cacheService ?? new CacheService();
            this.repository = repository ?? new DynamicRepository();
        }
        public SiteDB GetSiteDBConfig(int custId)
        {
            var siteDB = cacheService.GetOrSet(
                $"Alpha.CloneAccount.Services.Impl.SiteDBConfigs-{custId}",
                () => repository.Get<SiteDB>(new GetSiteDBConfigCriteria(custId)),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return siteDB;
        }

    }
}
