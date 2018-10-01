namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using DataAccess.Criteria.Customer;
    using DTO.Customer;
    using NAP.Framework.Data;
    using ViewModels;

    public class SuperSettingServiceImpl : CustomerSettingServiceImpl
    {
        private readonly AuthorizationService authorizationService;
        private readonly IDynamicRepository dynamicRepository;

        private readonly IReadOnlyDictionary<int, string> visibleBetSettingPermissions;

        public SuperSettingServiceImpl(
            IDynamicRepository dynamicRepository = null,
            AuthorizationService authorizationService = null,
            CustomerStatusQueryService customerStatusService = null,
            IReadOnlyDictionary<int, string> visibleBetSettingPermissions = null) : base(customerStatusService)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.visibleBetSettingPermissions = visibleBetSettingPermissions ?? AccountInfoProductConfiguration.VisibleBetSettingPermissions;
        }

        public override Setting GetSetting(
            int actorId,
            string custName,
            int uplineId,
            string groupName,
            DataSet customerInfoDataset)
        {
            var setting = base.GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);

            if (authorizationService.HasReadPermissionOnFunctions(
                visibleBetSettingPermissions.Values, actorId))
            {
                setting.Details.Insert(2, new ObjectInfo("BetSetting", "Betting Limit"));
            }

            setting.Details.Add(new ObjectInfo("DataRange", "Data Range"));
            return setting;
        }

        public override PersonalInfo GetPersonalInfo(int custId, DataSet customerInfoDataset)
        {
            var personalInfo = base.GetPersonalInfo(custId, customerInfoDataset);

            var superInfo = dynamicRepository.Get<CustomerInfo>(new GetSuperInfoCriteria(custId));

            personalInfo.Fax = ObjectInfo.CreateInstance("Fax", superInfo.Fax);
            personalInfo.Mobile = ObjectInfo.CreateInstance("Mobile", superInfo.MobilePhone);
            personalInfo.Phone = ObjectInfo.CreateInstance("Phone", superInfo.Phone);
            personalInfo.Name = ObjectInfo.CreateInstance("Name", $"{superInfo.FirstName} {superInfo.LastName}");

            return personalInfo;
        }
    }
}