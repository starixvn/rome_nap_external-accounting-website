namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using Core.Authorization;
    using Core.Shared;
    using Extensions;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomerModel(int actorId, string custName, string groupName, DataSet customerInfoDataset);
    }

    public class CustomerInfoQueryServiceImpl : CustomerInfoQueryService
    {
        private static readonly IReadOnlyDictionary<int, CustomerSettingQueryService> settingServiceInstances
            = new Dictionary<int, CustomerSettingQueryService>
            {
                [Role.Super] = new SuperSettingServiceImpl(),
                [Role.Master] = new MasterSettingServiceImpl(),
                [Role.Agent] = new CustomerSettingServiceImpl(),
                [Role.Member] = new MemberSettingServiceImpl()
            };

        protected readonly AuthorizationService authorizationService;
        protected readonly IReadOnlyDictionary<int, CustomerSettingQueryService> settingServices;

        public CustomerInfoQueryServiceImpl(
            AuthorizationService authorizationService = null,
            IReadOnlyDictionary<int, CustomerSettingQueryService> settingServices = null)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.settingServices = settingServices ?? settingServiceInstances;
        }

        public CustomerModel GetCustomerModel(int actorId, string custName, string groupName, DataSet customerInfoDataset)
        {
            var custInfoRow = customerInfoDataset.Tables[0].Rows[0];
            var roleId = custInfoRow.RoleId();
            var isUpdateCredit = authorizationService.HasUpdateCreditPermission(actorId);

            var customerModel = new CustomerModel(customerInfoDataset, isUpdateCredit, authorizationService.CanViewVNIP);

            AssignSettingValue(actorId, custName, groupName, customerInfoDataset, roleId, customerModel);

            return customerModel;
        }

        private void AssignSettingValue(
            int actorId,
            string custName,
            string groupName,
            DataSet customerInfoDataset,
            int roleId,
            CustomerModel customerModel)
        {
            var uplineId = roleId.IsSuper() ? 0 : (int) customerModel.PersonalInfo.UplineId.Value;

            if (!settingServices.ContainsKey(roleId))
            {
                throw new ArgumentException($"RoleId {roleId} is out of range");
            }

            customerModel.Setting =
                settingServices[roleId].GetSetting(actorId, custName, uplineId, groupName, customerInfoDataset);
        }
    }
}