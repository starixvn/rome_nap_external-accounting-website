namespace Fanex.ExternalAccount.DataAccess.Interface.Customer
{
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using System.Collections.Generic;
    using ViewModel.Customer.Suspend;

    public interface ICustomerDao
    {
        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId);

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
    }
}