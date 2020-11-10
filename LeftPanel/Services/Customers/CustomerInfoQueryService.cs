namespace Fanex.ExternalAccount.Module.LeftPanel.Services.Customers
{
    using Core.Authorization;
    using Core.Shared;
    using Extensions;
    using System;
    using System.Collections.Generic;
    using System.Data;
    using ViewModels;

    public interface CustomerInfoQueryService
    {
        CustomerModel GetCustomerModel(
            int actorId,
            string custName,
            string groupName,
            DataSet personalInfoDataset,
            DataSet loginInfoDataset,
            DataSet balanceInfoDataset,
            DataSet customerStatusInfoDataset,
            decimal balance,
            decimal member3rdPendingTransfer,
            decimal memberOustanding);
    }

    public class CustomerInfoQueryServiceImpl : CustomerInfoQueryService
    {
        protected readonly AuthorizationService authorizationService;
        protected IDictionary<int, CustomerSettingQueryService> settingServices;

        public CustomerInfoQueryServiceImpl(
            AuthorizationService authorizationService = null,
            IDictionary<int, CustomerSettingQueryService> settingServices = null)
        {
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.settingServices = settingServices;
        }

        public CustomerModel GetCustomerModel(
            int actorId,
            string custName,
            string groupName,
            DataSet personalInfoDataset,
            DataSet loginInfoDataset,
            DataSet balanceInfoDataset,
            DataSet customerStatusInfoDataset,
            decimal balance,
            decimal member3rdPendingTransfer,
            decimal memberOustanding)
        {
            var personalInfoRow = personalInfoDataset.Tables[0].Rows[0];
            var roleId = personalInfoRow.RoleId();
            var isUpdateCredit = authorizationService.HasUpdateCreditPermission(actorId);
            InstanceSettingService(roleId, actorId);
            var customerModel = new CustomerModel(
                personalInfoDataset,
                loginInfoDataset,
                balanceInfoDataset,
                customerStatusInfoDataset,
                isUpdateCredit,
                authorizationService.CanViewVNIP,
                balance,
                member3rdPendingTransfer,
                memberOustanding);

            AssignSettingValue(customerStatusInfoDataset, roleId, customerModel);
            customerModel.GroupName = groupName;

            return customerModel;
        }

        private void InstanceSettingService(int roleId, int userId)
        {
            settingServices = new Dictionary<int, CustomerSettingQueryService>
            {
                [Role.Super] = new SuperSettingServiceImpl(userId, roleId),
                [Role.Master] = new MasterSettingServiceImpl(userId, roleId),
                [Role.Agent] = new CustomerSettingServiceImpl(userId, roleId),
                [Role.Member] = new MemberSettingServiceImpl(userId, roleId)
            };
        }

        private void AssignSettingValue(
            DataSet customerStatusInfoDataset,
            int roleId,
            CustomerModel customerModel)
        {
            if (!settingServices.ContainsKey(roleId))
            {
                throw new ArgumentException($"RoleId {roleId} is out of range");
            }

            customerModel.Setting =
                settingServices[roleId].GetSetting(customerStatusInfoDataset);
        }
    }
}