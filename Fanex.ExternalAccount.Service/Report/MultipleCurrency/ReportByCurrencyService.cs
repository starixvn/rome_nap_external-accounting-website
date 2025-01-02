namespace Fanex.ExternalAccount.Service.Impl.Report.MultipleCurrency
{
    using Fanex.Caching;
    using Fanex.Caching.Memory;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Utilities;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public class ReportByCurrencyService : IReportByCurrencyService
    {
        private readonly IReportByCurrencyDAO _reportDao;
        private const string BalanceUpDayKey = "SUNPLUS.AGENT.MODEL.BL.REPORTS.BALANCEUPDAY";
        private static readonly ICacheService Cache = new CacheService(new MemoryCache(new MemoryCacheOptions()));

        public static DataTable GetBalanceUpDay()
        {
            Cache.TryGet<DataTable>(BalanceUpDayKey, out var balanceUpDay);
            if (balanceUpDay == null)
            {
                var reportDao = new ReportDao();
                balanceUpDay = reportDao.Age_SelBalanceUpDay();
                if (balanceUpDay != null)
                {
                    Cache.Set<DataTable>(BalanceUpDayKey, balanceUpDay, new CacheItemOptions { RelativeExpiration = TimeSpan.FromMinutes(5) });
                }
            }

            return balanceUpDay;
        }
        public static DateTime BalanceUpDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["BalanceUpDay"]);
        public static DateTime MoveBetsDate => Utils.ToDateTime(GetBalanceUpDay().Rows[0]["MoveBetsDay"]);

        public ReportByCurrencyService(
            IReportByCurrencyDAO reportRespository = null)
        {
            _reportDao = reportRespository ?? new ReportByCurrencyDAO();
        }
        public List<CurrencyEntity> GetCurrencyList(string groupName)
            => _reportDao.EAS_GetCurrencyList(groupName);

        public WinLossByCurrency GetSuperWinLossMCDetail(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencyIds,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_Com(groupname, custId, currencyIds, baseCurrencyName, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public WinLossByCurrency GetMasterWinLossMCDetail(
            string groupname,
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencyIds,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_SAg(groupname, custId, masterId, currencyIds, baseCurrencyName, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public WinLossByCurrencyMAg GetAgentWinLossMCDetail(
            string groupname,
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencyIds,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_MAg(groupname, custId, agentId, currencyIds, baseCurrencyName, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);

        public WinLossByCurrencyAg GetMemberWinLossMCDetail(
            string groupname,
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencyIds,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_Ag(groupname, custId, memberId, currencyIds, baseCurrencyName, fromDate, toDate, productIds, productDetailsXML, BalanceUpDate, MoveBetsDate);
    }

#pragma warning restore S109 // Magic numbers should not be used
}
