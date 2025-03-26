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

        public DateTime BalanceUpDate => GetBalanceDate().BalanceUpDate;

        public DateTime MoveBetsDate => GetBalanceDate().MoveBetsDate;

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
                var command = new GetBalanceDateCriteria();
                dynamicRepository.Execute(command);
                balanceDate = new BalanceDate {
                    BalanceUpDate = command.BalanceUpDate,
                    MoveBetsDate = command.MoveBetsDate
                };

                cacheService.Set<BalanceDate>(balanceDateCacheKey, balanceDate, new CacheItemOptions { RelativeExpiration = TimeSpan.FromMinutes(FiveMinutes) });
            }

            return balanceDate;
        }
    }

    public class BalanceDate
    {
        public DateTime MoveBetsDate { get; set; }

        public DateTime BalanceUpDate { get; set; }
    }

    public class GetBalanceDateCriteria : INonQueryCommand
    {
        public DateTime BalanceUpDate { get; set; }

        public DateTime MoveBetsDate { get; set; }

        public string GetSettingKey() => "Age_SelBalanceUpDay";

        public bool IsValid() => true;
    }
}
