namespace Fanex.ExternalAccount.Service.Interface.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using System.Collections.Generic;
    using ViewModel.Customer.Suspend;

    public interface ICustomerService
    {
        bool IsValidUsername(string userName, int uplineId);

        Customer UserInfo(int custId);

        Result EditUserInfo(Customer customer,
            string ip,
            int userId,
            int subAccId,
            string language);

        Result ResetPassword(ResetPasswordCriteria criteria);

        Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria);

        Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria);

        SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName);

        Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria);

        List<MultipleUpdateViewModel> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria);

        List<Result> SuspendMultipleCustomer(List<MultipleUpdateSuspendRequest> request, int userId, string ipv4);
    }
}