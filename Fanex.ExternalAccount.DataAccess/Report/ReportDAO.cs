namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Common.Extensions;
    using DTO.Customer;
    using DTO.Report;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    using Interface.Report;
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Globalization;
    using System.Linq;

    public partial class ReportDao : IReportDao
    {
        private IDynamicRepository dynamicRepository;

        public ReportDao() : this(null)
        { }

        public ReportDao(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public LineInfo GetCustomerInfo(GetCustomerInfoCriteria criteria)
            => dynamicRepository.Get<LineInfo>(criteria);

        public IEnumerable<SuggestedUserName> EAS_CustomerSelUserSearch(GetSuggestUsernamesCriteria criteria)
        {
            var result = dynamicRepository.Fetch<SuggestedUserName>(criteria);

            if (result.Any())
            {
                this.UpdateSearchedUserName(new UpdateSuggestUsernameCriteria(criteria.CustId, criteria.UserName.ToUpper(CultureInfo.InvariantCulture), criteria.GroupName));
            }
            
            return result;
        }

        private void UpdateSearchedUserName(UpdateSuggestUsernameCriteria criteria)
            => dynamicRepository.Execute(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(GetCompanyDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(GetSuperDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(GetMasterDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(GetAgentDailyWinlossCriteria criteria)
            => dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(GetSuperWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetailExtra>(criteria);


        public IEnumerable<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(GetMasterWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetail>(criteria);

        public IEnumerable<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(GetAgentWinlossDetailCriteria criteria)
            => dynamicRepository.Fetch<WinLossDetailMAg>(criteria);

        public IEnumerable<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(GetMemberWinlossDetailCriteria criteria)
             => dynamicRepository.Fetch<WinLossDetailAg>(criteria);

        public IEnumerable<int> EAS_Rpt_WinLossDaily_SortingOrder(GetSortingOrderCriteria criteria)
            => dynamicRepository.Fetch<int>(criteria);

        public IEnumerable<ExtraPTModel> EAS_Rpt_ViewExtraPT2(GetExtraPTCriteria criteria)
            => dynamicRepository.Fetch<ExtraPTModel>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Com(GetSuperWinlossDetailMultiCurrencyCriteria criteria)
            => dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_SAg(GetMasterWinlossDetailMultiCurrencyCriteria criteria)
            => dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);
        
        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_MAg(GetAgentWinlossDetailMultiCurrencyCriteria criteria)
            => dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Ag(GetMemberWinlossDetailMultiCurrencyCriteria criteria)
            => dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);
    }
}