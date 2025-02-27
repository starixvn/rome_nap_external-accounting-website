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

        IEnumerable<int> EAS_Rpt_WinLossDaily_SortingOrder(GetSortingOrderCriteria criteria);

        IEnumerable<ExtraPTModel> EAS_Rpt_ViewExtraPT2(GetExtraPTCriteria criteria);

        IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Com(GetSuperWinlossDetailMultiCurrencyCriteria criteria);

        IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_SAg(GetMasterWinlossDetailMultiCurrencyCriteria criteria);

        IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_MAg(GetAgentWinlossDetailMultiCurrencyCriteria criteria);

        IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Ag(GetMemberWinlossDetailMultiCurrencyCriteria criteria);
    }
}