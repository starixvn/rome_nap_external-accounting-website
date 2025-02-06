namespace Fanex.ExternalAccount.DataAccess.Interface.Report
{
    using DTO.Customer;
    using DTO.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public interface IReportDao
    {
        DataTable Age_SelBalanceUpDay();

        DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        LineInfo GetCustomerInfo(string groupname, int custId, string userName, bool isGetUplineInfo);

        List<SuggestedUserName> EAS_CustomerSelUserSearch(int custId, string username, string groupname);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com14(DateTime winlostDate, int custId, string groupname);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(DateTime winlostDate, int custId, string groupname);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg14(DateTime winlostDate, int custId, int masterId, int memberId);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(DateTime winlostDate, int custId, int masterId, int memberId);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg14(DateTime winlostDate, int custId, int agentId, int memberId);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(DateTime winlostDate, int custId, int agentId, int memberId);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag14(DateTime winlostDate, int custId, int memberId);

        List<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(DateTime winlostDate, int custId, int memberId);

        List<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(
            string groupname,
            int custId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(
            int custId,
            int masterId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(
            int custId,
            int agentId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(
            int custId,
            int memberId,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            DateTime movebetDay);

        ArrayList EAS_Rpt_WinLossDaily_SortingOrder(string groupname);

        List<ExtraPTModel> EAS_Rpt_ViewExtraPT2(string groupName);


#pragma warning disable S107 // Methods should not have too many parameters
        List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Com(
            string groupname,
            int custId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_SAg(
            int custId,
            int masterId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_MAg(
            int custId,
            int agentId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);

        List<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Ag(
            int custId,
            int memberId,
            string currencyIds,
            int baseCurrency,
            DateTime fromDate,
            DateTime toDate,
            string productIds,
            string productDetailsXML,
            DateTime balanceUpDay,
            DateTime moveBetsDay);
#pragma warning restore S107 // Methods should not have too many parameters
    }
}