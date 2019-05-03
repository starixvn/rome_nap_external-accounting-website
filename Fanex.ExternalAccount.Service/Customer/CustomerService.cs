namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using Interface.Customer;
    using System.Collections.Generic;
    using System.Text;
    using ViewModel.Customer.Suspend;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDao dataAccess;

        public CustomerService(ICustomerDao dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new CustomerDao();
        }

        public bool IsValidUsername(string userName, int uplineId)
            => dataAccess.IsValidUsername(userName, uplineId);

        public Customer UserInfo(int custId) => dataAccess.UserInfo(custId);

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
            => dataAccess.EditUserInfo(customer, ip, userId, subAccId, language);


        public Result ResetPassword(ResetPasswordCriteria criteria)
            => dataAccess.ResetPassword(criteria);

        public Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria)
            => dataAccess.ConfigDoubleComm(criteria);

        public Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria)
            => dataAccess.CheckNewCustomerIsReachLimitation(criteria);

        public SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName)
            => new SingleUpdateViewModel(custId, userName, dataAccess.GetSingleUpdateSuspendAffectedDownlines(new GetSingleSuspendCustomerInfoCriteria(custId)));

        public Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria)
            => dataAccess.SuspendCustomer(criteria);

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
            => dataAccess.GetSuspendDownlinesDetail(criteria);

        public List<MultipleUpdateViewModel> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
            => dataAccess.GetMultipleSuspendCustomerViewModel(criteria);

        public List<Result> SuspendMultipleCustomer(List<MultipleUpdateSuspendRequest> request, int userId, string ipv4)
        {
            var result = new List<Result>();
            var successUpdateSuspendedList = new StringBuilder();
            var successUpdateUnsuspendedList = new StringBuilder();
            foreach (var requestItem in request)
            {
                var criteria = new UpdateSuspendCustomerCriteria(userId, requestItem.CustId, requestItem.Suspended, "E", ipv4);
                var preResult = dataAccess.SuspendCustomer(criteria);
                if(preResult.IsSuccess)
                {
                    if(requestItem.Suspended)
                    {
                        successUpdateSuspendedList.Append(requestItem.UserName);
                        successUpdateSuspendedList.Append(", ");
                    }
                    else
                    {
                        successUpdateUnsuspendedList.Append(requestItem.UserName);
                        successUpdateUnsuspendedList.Append(", ");
                    }
                }
                else
                {
                    result.Add(new Result(preResult.ErrorCode, $"{requestItem.UserName}: {preResult.ErrorMessage}"));
                }
            }

            if (successUpdateSuspendedList.Length > 0)
            {
                successUpdateSuspendedList.Length -= 2;
                result.Insert(0, new Result(0, $"Status of {successUpdateSuspendedList.ToString()} and their affected downlines have been successfully updated to Suspended."));
            }

            if (successUpdateUnsuspendedList.Length > 0)
            {
                successUpdateUnsuspendedList.Length -= 2;
                result.Insert(0, new Result(0, $"Status of {successUpdateUnsuspendedList.ToString()} and their affected downlines have been successfully updated to Unsuspended."));
            }

            return result;
        }
    }
}