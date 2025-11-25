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
        bool IsArchivedCustId(int custId);

        Customer UserInfo(int custId, byte settingStatus);

        Result EditUserInfo(Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCommand criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCommand criteria);

        void KickoutCustomer(int custId, byte roleId, string ip, string mode = null);

        SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName);

        Result SuspendCustomer(UpdateSuspendCustomerCommand criteria);

        List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria);

        List<Result> SuspendMultipleCustomer(List<MultipleUpdateSuspendRequest> request, int userId, string ipv4, int roleId);

        CustomerStatusDTO GetCustomerAllStatus(int custId, byte settingStatus);

        Result CheckLimitNewDownline(int custId, int roleId);

        SingleOpenCustomerViewModel GetSingleOpenCustomerInfo(GetSingleOpenCustomerInfoQuery query);

        MultipleOpenCustomerViewModel GetMultipleOpenCustomerInfo(GetMultipleOpenCustomerInfoQuery query);

        SingleOpenDownlinesViewModel GetSingleOpenDownlinesInfo(GetSingleOpenDownlinesInfoQuery query);

        Result UpdateSingleCustomerClosedStatus(UpdateSingleClosedStatusQuery query);

        IEnumerable<Result> UpdateMultipleCustomerClosedStatus(IEnumerable<UpdateMultipleClosedStatusQuery> query, int roleId);

        IEnumerable<CustomerProtectedStatus> GetMultipleProtectedStatus(string groupName, string custIds, byte roleId, int userId);

        IEnumerable<Result> UpdateMultipleProtectedStatus(CustomerProtectedStatus[] protectedStatus, int userId, string ip);

        Result UpdateProtectedStatus(int custId, byte roleId, bool isProtected, int userId, string ip);

        Result CheckMultipleSupersBetSetting(string sourceUser, string destUsers, string groupName, int userId, bool isCopy = true);

        SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria);
    }
}