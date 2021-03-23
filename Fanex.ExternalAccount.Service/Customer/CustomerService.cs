namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System.Collections.Generic;
    using System.Text;
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Utilities;
    using Interface.Customer;
    using ViewModel.Customer.Suspend;

    public class CustomerService : ICustomerService
    {
        private readonly ICustomerDao dataAccess;
        public CustomerService() : this(null)
        {
        }

        public CustomerService(ICustomerDao dataAccess)
        {
            this.dataAccess = dataAccess ?? new CustomerDao();
        }

        public bool IsValidUsername(string userName, int uplineId)
            => dataAccess.IsValidUsername(userName, uplineId);

        public Customer UserInfo(int custId, byte settingStatus) => dataAccess.UserInfo(custId, settingStatus);

        public CustomerStatusDTO GetCustomerAllStatus(int custId, byte settingStatus) 
            => dataAccess.GetCustomerAllStatus(new GetCustomerAllStatusCriteria { CustId = custId, SettingStatus = settingStatus });

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
            => dataAccess.EditUserInfo(customer, ip, userId, subAccId, language);


        public Result ResetPassword(ResetPasswordCriteria criteria)
            => dataAccess.ResetPassword(criteria);

        public Result ConfigDoubleComm(UpdateDoubleCommCriteria criteria)
            => dataAccess.ConfigDoubleComm(criteria);

        public Result CheckNewCustomerIsReachLimitation(CheckNewCustomerLimitationCriteria criteria)
            => dataAccess.CheckNewCustomerIsReachLimitation(criteria);

        public void KickoutCustomer(int custId, byte roleId, string ip)
            => dataAccess.KickoutCustomer(new KickoutCustomerCriteria(custId, roleId, ip));

        public SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName)
            => new SingleUpdateViewModel(custId, userName, dataAccess.GetSingleUpdateSuspendAffectedDownlines(new GetSingleSuspendCustomerInfoCriteria(custId)));

        public Result SuspendCustomer(UpdateSuspendCustomerCriteria criteria)
            => dataAccess.SuspendCustomer(criteria);

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
            => dataAccess.GetSuspendDownlinesDetail(criteria);

        public List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
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
                if (preResult.IsSuccess)
                {
                    if (requestItem.Suspended)
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
#pragma warning disable S109 // Magic numbers should not be used
                successUpdateSuspendedList.Length -= 2;
#pragma warning restore S109 // Magic numbers should not be used
                result.Insert(0, new Result(0, $"Status of {successUpdateSuspendedList.ToString()} and their affected downlines have been successfully updated to Suspended."));
            }

            if (successUpdateUnsuspendedList.Length > 0)
            {
#pragma warning disable S109 // Magic numbers should not be used
                successUpdateUnsuspendedList.Length -= 2;
#pragma warning restore S109 // Magic numbers should not be used
                result.Insert(0, new Result(0, $"Status of {successUpdateUnsuspendedList.ToString()} and their affected downlines have been successfully updated to Unsuspended."));
            }

            return result;
        }

        public Result CheckLimitNewDownline(int custId, int roleId)
        {
            var result = new Result();
            var isAllow = dataAccess.CheckLimitNewDownline(new CheckLimitNewDownlineCriteria(custId));
            
            if (!isAllow)
            {
                result.ErrorCode = "1001";

                if (roleId == Role.Super)
                {
                    result.ErrorMessage = ViewLang.LimitSuperNewDownlineError;
                }
                else if (roleId == Role.Master)
                {
                    result.ErrorMessage = ViewLang.LimitMasterNewDownlineError;
                }
                else if (roleId == Role.Agent)
                {
                    result.ErrorMessage = ViewLang.LimitAgentNewDownlineError;
                }
            }

            return result;
        }
    }
}