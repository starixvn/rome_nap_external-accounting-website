namespace Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency
{
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;

    public interface IReportByCurrencyService
    {

        List<CurrencyEntity> GetCurrencyList(string groupName);

#pragma warning disable S107 // Methods should not have too many parameters
        WinLossByCurrency GetSuperWinLossMCDetail(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName);

        WinLossByCurrency GetMasterWinLossMCDetail(
            string groupname,
            int custId,
            int masterId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName);

        WinLossByCurrencyMAg GetAgentWinLossMCDetail(
            string groupname,
            int custId,
            int agentId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName);

        WinLossByCurrencyAg GetMemberWinLossMCDetail(
            string groupname,
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            string currencies,
            string baseCurrencyName);
#pragma warning restore S107 // Methods should not have too many parameters
    }
}
