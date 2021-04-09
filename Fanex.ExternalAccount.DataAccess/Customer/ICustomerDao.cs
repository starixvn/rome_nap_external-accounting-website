namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using System;
    using System.Collections.Generic;
    using ViewModel.Customer.Suspend;

    public interface ICustomerDao
    {
        Customer GetCustomerByUserName(string userName, string groupName, int userId);

        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId, byte settingStatus);

        Result EditUserInfo(
            Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCriteria criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria);

        Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria);

        void KickoutCustomer(KickoutCustomerCriteria criteria);

        List<SingleUpdateAffectedDownline> GetSingleUpdateSuspendAffectedDownlines(GetSingleSuspendCustomerInfoCriteria criteria);

        Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria);

        SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria);

        List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria);

        CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria);

        bool CheckLimitNewDownline(CheckLimitNewDownlineCriteria criteria);

        IEnumerable<SingleOpenAffectedDownline> GetSingleOpenAffectedDownlines(GetSingleOpenCustomerInfoCriteria criteria);

        Tuple<int, List<SingleOpenDownline>> GetSingleOpenDownlines(GetSingleOpenDownlinesInfoCriteria criteria);

        Result UpdateCustomerClosedStatus(UpdateClosedStatusCriteria criteria);
    }
}