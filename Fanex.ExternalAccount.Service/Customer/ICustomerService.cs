namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Requests.Customer.OpenCustomer;
    using System.Collections.Generic;
    using ViewModel.Customer.OpenCustomer;
    using ViewModel.Customer.Suspend;

    public interface ICustomerService
    {
        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId, byte settingStatus);

        Result EditUserInfo(Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCriteria criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria);

        Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria);

        void KickoutCustomer(int custId, byte roleId, string ip);

        SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName);

        Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria);

        List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria);

        List<Result> SuspendMultipleCustomer(List<MultipleUpdateSuspendRequest> request, int userId, string ipv4);

        CustomerStatusDTO GetCustomerAllStatus(int custId, byte settingStatus);

        Result CheckLimitNewDownline(int custId, int roleId);

        SingleOpenCustomerViewModel GetSingleOpenCustomerInfo(GetSingleOpenCustomerInfoQuery query);

        MultipleOpenCustomerViewModel GetMultipleOpenCustomerInfo(GetMultipleOpenCustomerInfoQuery query);

        SingleOpenDownlinesViewModel GetSingleOpenDownlinesInfo(GetSingleOpenDownlinesInfoQuery query);

        Result UpdateSingleCustomerClosedStatus(UpdateSingleClosedStatusQuery query);

        IEnumerable<Result> UpdateMultipleCustomerClosedStatus(IEnumerable<UpdateMultipleClosedStatusQuery> query);
    }
}