namespace Fanex.ExternalAccount.Service.ViewMaxPayoutSettingInformation
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.ViewMaxPayoutSettingInformation;
    using Fanex.ExternalAccount.ViewModel.ViewMaxPayoutSettingInformation;

    public class ViewMaxPayoutSettingInformationService
    {
        private readonly IDynamicRepository _dynamicRepository;

        public ViewMaxPayoutSettingInformationService() : this(null) { }
        public ViewMaxPayoutSettingInformationService(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<SabaSportViewModel> GetSabaSports(string site)
            => _dynamicRepository.Fetch<SabaSportViewModel>(new GetSabaSportsCriteria(site));
    }
}
