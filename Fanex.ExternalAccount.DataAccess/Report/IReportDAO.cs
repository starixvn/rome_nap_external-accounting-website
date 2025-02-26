namespace Fanex.ExternalAccount.DataAccess.Interface.Report
{
    using DTO.Customer;
    using DTO.Report;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;

    public interface IReportDao
    {
        LineInfo GetCustomerInfo(GetCustomerInfoCriteria criteria);

        IEnumerable<SuggestedUserName> EAS_CustomerSelUserSearch(GetSuggestUsernamesCriteria criteria);

        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(GetCompanyDailyWinlossCriteria criteria);

        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(GetSuperDailyWinlossCriteria criteria);

        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(GetMasterDailyWinlossCriteria criteria);

        IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(GetAgentDailyWinlossCriteria criteria);

        IEnumerable<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(GetSuperWinlossDetailCriteria criteria);

        IEnumerable<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(GetMasterWinlossDetailCriteria criteria);
            int memberId,

        IEnumerable<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(GetAgentWinlossDetailCriteria criteria);
            int memberId,

        IEnumerable<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(GetMemberWinlossDetailCriteria criteria);

        List<WinLossDetailIBCBetLiveCasino> EAS_Rpt_WinLossDetailIBCBetLiveCasino_Com(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            DateTime movebetDay);

        IEnumerable<int> EAS_Rpt_WinLossDaily_SortingOrder(GetSortingOrderCriteria criteria);

        IEnumerable<ExtraPTModel> EAS_Rpt_ViewExtraPT2(GetExtraPTCriteria criteria);


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