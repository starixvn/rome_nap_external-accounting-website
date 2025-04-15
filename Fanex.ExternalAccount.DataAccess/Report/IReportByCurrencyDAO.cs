namespace Fanex.ExternalAccount.DataAccess.Interface.Report
{
    using Fanex.ExternalAccount.DTO.Currency;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;

    public interface IReportByCurrencyDAO
    {
        List<CurrencyEntity> EAS_GetCurrencyList(string groupName);

#pragma warning disable S107 // Methods should not have too many parameters
        WinLossByCurrency EAS_Rpt_WinLossByCurDaily_Com(
            string groupname,
            int custId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        WinLossByCurrency EAS_Rpt_WinLossByCurDaily_SAg(
            string groupname,
            int custId,
            int masterId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);
        
        WinLossByCurrencyMAg EAS_Rpt_WinLossByCurDaily_MAg(
            string groupname,
            int custId,
            int agentId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);      

        WinLossByCurrencyAg EAS_Rpt_WinLossByCurDaily_Ag(
            string groupname,
            int custId,
            int memberId,
            string currencies,
            string baseCurrencyName,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);
#pragma warning restore S107 // Methods should not have too many parameters
    }
}
