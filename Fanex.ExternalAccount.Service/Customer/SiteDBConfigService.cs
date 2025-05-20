using Fanex.Caching;
using Fanex.Data.Repository;
using Fanex.ExternalAccount.DataAccess.Customer;
using Fanex.ExternalAccount.DTO;
using System;

namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    public interface ISiteDBConfigService
    {
        SiteDB GetSiteDBConfigBySite(string site);

        SiteDB GetSiteDBConfigByCustId(int custId);

        SiteDB GetSiteDBConfigByUserName(string userName);
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

        public SiteDB GetSiteDBConfigBySite(string site)
        {
            var siteDB = cacheService.GetOrSet(
                $"CustDBConfigs-{site}",
                () => repository.Get<SiteDB>(new GetCustDBConfigBySiteCriteria(site: site)),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return siteDB;
        }

        public SiteDB GetSiteDBConfigByCustId(int custId)
        {
            var siteDB = cacheService.GetOrSet(
                $"CustDBConfigs-{custId}",
                () => repository.Get<SiteDB>(new GetCustDBConfigByCustCriteria(custId: custId)),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return siteDB;
        }

        public SiteDB GetSiteDBConfigByUserName(string userName)
        {
            var siteDB = cacheService.GetOrSet(
                $"CustDBConfigs-{userName}",
                () => repository.Get<SiteDB>(new GetCustDBConfigByCustCriteria(userName: userName)),
                new CacheItemOptions().SetAbsoluteExpiration(TimeSpan.FromDays(1)));

            return siteDB;
        }
    }
}
