namespace Fanex.ExternalAccount.Core.Shared
{
    using Fanex.Caching;
    using Fanex.Data.Repository;
    using System;

    public interface IReportUtils
    {
        DateTime BalanceUpDate { get; }

        DateTime MoveBetsDate { get; }

        BalanceDate GetBalanceDate();
    }

    public class ReportUtils : IReportUtils
    {
        private const int FiveMinutes = 5;
        private readonly IDynamicRepository dynamicRepository;

        private readonly ICacheService cacheService;

        private readonly string balanceDateCacheKey = "BalanceDateCacheKey";

        public DateTime BalanceUpDate => GetBalanceDate().BalanceUpDay;

        public DateTime MoveBetsDate => GetBalanceDate().MoveBetsDay;

        public ReportUtils(): this(null, null)
        { }

        public ReportUtils(IDynamicRepository dynamicRepository, ICacheService cacheService)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.cacheService = cacheService ?? new CacheService();
        }

        public BalanceDate GetBalanceDate()
        {
            var balanceDate = cacheService.Get<BalanceDate>(balanceDateCacheKey);
            if (balanceDate == null)
            {
                balanceDate= dynamicRepository.Get<BalanceDate>(new GetBalanceDateCriteria());
                cacheService.Set<BalanceDate>(balanceDateCacheKey, balanceDate, new CacheItemOptions { RelativeExpiration = TimeSpan.FromMinutes(FiveMinutes) });
            }

            return balanceDate;
        }
    }

    public class BalanceDate
    {
        public DateTime MoveBetsDay { get; set; }

        public DateTime BalanceUpDay { get; set; }
    }

    public class GetBalanceDateCriteria : CriteriaBase
    {
        public override string GetSettingKey() => "Age_SelBalanceUpDay";

        public override bool IsValid() => true;
    }
}
