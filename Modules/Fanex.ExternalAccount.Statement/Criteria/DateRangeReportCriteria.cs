namespace Fanex.ExternalAccount.Statement.Criteria.StatementReport
{
    using Fanex.Caching;
    // using Fanex.Caching.Memory;
    // using Fanex.CustomerSetting.Module.Core;
    // using Fanex.ExternalAccount.Statement.Repositories.StatementReport;
    //using Fanex.NAP.Framework.Data;
    using System;
    using System.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Core.Shared;
    public class DateRangeReportCriteria : CriteriaBase
    {
        private const string BalanceUpDayKey = "__balanceUpDayKey";
        private static ICacheService _cache;
        private DataTable _balanceUpDay;
        private readonly IReportUtils _reportUtils;

        protected DateRangeReportCriteria(IReportUtils reportUtils = null) => _reportUtils = reportUtils ?? new ReportUtils();
        
        public DateTime FromDate { get; set; }

        public DateTime ToDate { get; set; }

        public DateTime BalanceUpDay =>  _reportUtils.BalanceUpDate;

        public DateTime MoveBetsDay =>  _reportUtils.MoveBetsDate;

        public override string GetSettingKey()
        {
            throw new NotImplementedException();
        }

        public override bool IsValid()
        {
            return this.FromDate.Year >= 1753 &&
                this.FromDate.Year <= 9999 &&
                this.ToDate.Year >= 1753 &&
                this.ToDate.Year <= 9999 &&
                this.ToDate >= this.FromDate;
        }

        // private DataTable GetBalanceUpDay()
        // {
        //     _report = new StatementReportRepository();
        //
        //     if (_cache == null)
        //     {
        //         _cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));
        //     }
        //
        //     _balanceUpDay = _cache.Get<DataTable>(BalanceUpDayKey);
        //
        //     if (_balanceUpDay == null)
        //     {
        //         _balanceUpDay = _report.BalanceUpDate();
        //         _cache.Set<DataTable>(BalanceUpDayKey, _balanceUpDay, new CacheItemOptions().SetAbsoluteExpiration(DateTime.Now.AddMinutes(5)));
        //
        //         return _balanceUpDay;
        //     }
        //
        //     return _balanceUpDay;
        // }
    }
}
