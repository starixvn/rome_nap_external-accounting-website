namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using System;
    using System.Collections.Generic;
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.ExternalAccount.ViewModel.Customer.Suspend;

    public interface ICustomerDao
    {
        Customer GetCustomerByUserName(string userName, string groupName, int userId);

        bool IsArchivedCustId(int custId);

        Customer UserInfo(int custId, byte settingStatus);

        Result EditUserInfo(
            Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCommand command);

        Result ConfigDoubleComm(UpdateDoubleCommCommand command);

        void KickoutCustomer(KickoutCustomerCommand command);

        IEnumerable<SingleUpdateAffectedDownline> GetSingleUpdateSuspendAffectedDownlines(GetSingleSuspendCustomerInfoCriteria criteria);

        Result SuspendCustomer(UpdateSuspendCustomerCommand command);

        SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria);

        List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria);

        CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria);

        bool CheckLimitNewDownline(CheckLimitNewDownlineCommand command);

        IEnumerable<SingleOpenAffectedDownline> GetSingleOpenAffectedDownlines(GetSingleOpenCustomerInfoCriteria criteria);

        IEnumerable<CustomerClosedStatusRow> GetCustomerClosedStatusList(GetMultipleOpenCustomerInfoCriteria criteria);

        Tuple<int, List<SingleOpenDownline>> GetSingleOpenDownlines(GetSingleOpenDownlinesInfoCriteria criteria);

        Result UpdateCustomerClosedStatus(UpdateClosedStatusCriteria criteria);

        IEnumerable<CustomerProtectedStatus> GetMultipleProtectedStatus(GetMultipleProtectedStatusCriteria criteria);

        Result UpdateProtectedStatus(UpdateProtectedStatusCriteria criteria);

        Result CheckMultipleSupersBetSetting(CheckMultipleSupersBetSettingCriteria criteria);
    }
}