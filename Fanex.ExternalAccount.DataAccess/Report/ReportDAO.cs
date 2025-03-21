namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using DTO.Customer;
    using DTO.Report;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.Report;
    using Interface.Report;

    public partial class ReportDao : IReportDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public ReportDao() : this(null)
        { }

        public ReportDao(IDynamicRepository dynamicRepository)
        {
            this._dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public LineInfo GetCustomerInfo(GetCustomerInfoCriteria criteria)
            => _dynamicRepository.Get<LineInfo>(criteria);

        public IEnumerable<SuggestedUserName> EAS_CustomerSelUserSearch(GetSuggestUsernamesCriteria criteria)
        {
            var result = _dynamicRepository.Fetch<SuggestedUserName>(criteria);

            if (result.Any())
            {
                this.UpdateSearchedUserName(new UpdateSuggestUsernameCriteria(criteria.CustId, criteria.UserName.ToUpper(CultureInfo.InvariantCulture), criteria.GroupName));
            }
            
            return result;
        }

        private void UpdateSearchedUserName(UpdateSuggestUsernameCriteria criteria)
            => _dynamicRepository.Execute(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Com(GetCompanyDailyWinlossCriteria criteria)
            => _dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_SAg(GetSuperDailyWinlossCriteria criteria)
            => _dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_MAg(GetMasterDailyWinlossCriteria criteria)
            => _dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<DailyWinLoss> EAS_Rpt_WinLossDaily_Ag(GetAgentDailyWinlossCriteria criteria)
            => _dynamicRepository.Fetch<DailyWinLoss>(criteria);

        public IEnumerable<WinLossDetailExtra> EAS_Rpt_WinLossDetail_Com(GetSuperWinlossDetailCriteria criteria)
            => _dynamicRepository.Fetch<WinLossDetailExtra>(criteria);


        public IEnumerable<WinLossDetail> EAS_Rpt_WinLossDetail_SAg(GetMasterWinlossDetailCriteria criteria)
            => _dynamicRepository.Fetch<WinLossDetail>(criteria);

        public IEnumerable<WinLossDetailMAg> EAS_Rpt_WinLossDetail_MAg(GetAgentWinlossDetailCriteria criteria)
            => _dynamicRepository.Fetch<WinLossDetailMAg>(criteria);

        public IEnumerable<WinLossDetailAg> EAS_Rpt_WinLossDetail_Ag(GetMemberWinlossDetailCriteria criteria)
             => _dynamicRepository.Fetch<WinLossDetailAg>(criteria);

        public IEnumerable<int> EAS_Rpt_WinLossDaily_SortingOrder(GetSortingOrderCriteria criteria)
            => _dynamicRepository.Fetch<int>(criteria);

        public IEnumerable<ExtraPTModel> EAS_Rpt_ViewExtraPT2(GetExtraPTCriteria criteria)
            => _dynamicRepository.Fetch<ExtraPTModel>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Com(GetSuperWinlossDetailMultiCurrencyCriteria criteria)
            => _dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_SAg(GetMasterWinlossDetailMultiCurrencyCriteria criteria)
            => _dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);
        
        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_MAg(GetAgentWinlossDetailMultiCurrencyCriteria criteria)
            => _dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);

        public IEnumerable<WinLossByCurrencyDetail> EAS_Rpt_WinLossMCDetail_Ag(GetMemberWinlossDetailMultiCurrencyCriteria criteria)
            => _dynamicRepository.Fetch<WinLossByCurrencyDetail>(criteria);
    }
}