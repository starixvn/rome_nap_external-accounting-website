namespace Fanex.ExternalAccount.Service.Impl.Report.MultipleCurrency
{
    using System;
    using System.Collections.Generic;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DataAccess.Interface.Report;
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency;

    public class ReportByCurrencyService : IReportByCurrencyService
    {
        private readonly IReportByCurrencyDAO _reportDao;
        private readonly IReportUtils _reportUtils;
        
        public ReportByCurrencyService(
            IReportByCurrencyDAO reportRespository = null,
            IReportUtils reportUtils = null)
        {
            _reportDao = reportRespository ?? new ReportByCurrencyDAO();
            _reportUtils = reportUtils ?? new ReportUtils();
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
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_Com(groupname, custId, currencies, baseCurrencyName, fromDate, 
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);

        public WinLossByCurrency GetMasterWinLossMCDetail(
            string groupname,
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_SAg(groupname, custId, masterId, 0, currencies, baseCurrencyName, fromDate, 
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);
        
        public WinLossByCurrency GetDirectMemberOfSuperWinLossMCDetail(
            string groupname,
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_SAg(groupname, custId, 0,memberId, currencies, baseCurrencyName, fromDate, 
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);

        public WinLossByCurrencyMAg GetAgentWinLossMCDetail(
            string groupname,
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_MAg(groupname, custId, agentId, 0, currencies, baseCurrencyName, fromDate,
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);
        
        public WinLossByCurrencyMAg GetDirectMemberOfMasterWinLossMCDetail(
            string groupname,
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_MAg(groupname, custId, 0,memberId, currencies, baseCurrencyName, fromDate,
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);

        public WinLossByCurrencyAg GetMemberWinLossMCDetail(
            string groupname,
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName)
            => _reportDao.EAS_Rpt_WinLossByCurDaily_Ag(groupname, custId, memberId, currencies, baseCurrencyName, fromDate,
                toDate, productIds, productDetailsXML, _reportUtils.BalanceUpDate, _reportUtils.MoveBetsDate);
    }

#pragma warning restore S109 // Magic numbers should not be used
}
