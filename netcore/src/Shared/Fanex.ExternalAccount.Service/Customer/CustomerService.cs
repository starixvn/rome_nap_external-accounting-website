namespace Fanex.ExternalAccount.Service.Impl.Customer
{
    using System;
    using System.Collections.Generic;
    using System.Text;
    using Core.Shared;
    using DataAccess.Criteria.Customer;
    using DataAccess.Criteria.Customer.CustomerStatus;
    using DataAccess.Impl.Customer;
    using DataAccess.Interface.Customer;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.Logging.Extensions.Web;
    using Interface.Customer;
    using Requests.Customer.OpenCustomer;
    using Utilities;
    using ViewModel.Customer.OpenCustomer;
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

        public bool IsArchivedCustId(int custId)
            => dataAccess.IsArchivedCustId(custId);

        public Customer UserInfo(int custId, byte settingStatus) => dataAccess.UserInfo(custId, settingStatus);

        public CustomerStatusDTO GetCustomerAllStatus(int custId, byte settingStatus)
            => dataAccess.GetCustomerAllStatus(new GetCustomerAllStatusCriteria { CustId = custId, SettingStatus = settingStatus });

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
            => dataAccess.EditUserInfo(customer, ip, userId, subAccId, language);

        public Result ResetPassword(ResetPasswordCommand criteria)
            => dataAccess.ResetPassword(criteria);

        public Result ConfigDoubleComm(UpdateDoubleCommCommand criteria)
            => dataAccess.ConfigDoubleComm(criteria);

        public void KickoutCustomer(int custId, byte roleId, string ip, string mode = null)
        {
            if (Config.IsEnabledRecheckin)
            {
                try
                {
                    dataAccess.KickoutCustomer(new KickoutCustomerCommand(custId, roleId, ip, mode));
                }
                catch (Exception ex)
                {
                    ExceptionHandler.Handle(ex);
                }
            }
        }

        public SingleUpdateViewModel GetSingleUpdateSuspendViewModel(int custId, string userName)
            => new SingleUpdateViewModel(custId, userName, dataAccess.GetSingleUpdateSuspendAffectedDownlines(new GetSingleSuspendCustomerInfoCriteria(custId)));

        public Result SuspendCustomer(UpdateSuspendCustomerCommand criteria)
            => dataAccess.SuspendCustomer(criteria);

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
            => dataAccess.GetSuspendDownlinesDetail(criteria);

        public List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
            => dataAccess.GetMultipleSuspendCustomerViewModel(criteria);

        public List<Result> SuspendMultipleCustomer(List<MultipleUpdateSuspendRequest> request, int userId, string ipv4, int roleId)
        {
            var result = new List<Result>();
            var successUpdateSuspendedList = new StringBuilder();
            var successUpdateUnsuspendedList = new StringBuilder();
            foreach (var requestItem in request)
            {
                var criteria = new UpdateSuspendCustomerCommand(userId, requestItem.CustId, requestItem.Suspended, "E", ipv4);
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

            const int indent = 2;

            if (successUpdateSuspendedList.Length > 0)
            {
                successUpdateSuspendedList.Length -= indent;
                string message = roleId != Role.Member
                    ? $"Status of {successUpdateSuspendedList.ToString()} and their affected downlines have been successfully updated to Suspended."
                    : $"{successUpdateSuspendedList.ToString()} have been successfully suspended.";
                result.Insert(0, new Result(0, message));
            }

            if (successUpdateUnsuspendedList.Length > 0)
            {
                successUpdateUnsuspendedList.Length -= indent;
                string message = roleId != Role.Member
                    ? $"Status of {successUpdateUnsuspendedList.ToString()} and their affected downlines have been successfully updated to Unsuspended."
                    : $"{successUpdateUnsuspendedList.ToString()} have been successfully unsuspended.";
                result.Insert(0, new Result(0, message));
            }

            return result;
        }

        public Result CheckLimitNewDownline(int custId, int roleId)
        {
            var result = new Result();
            var isAllow = dataAccess.CheckLimitNewDownline(new CheckLimitNewDownlineCommand(custId));

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

        public SingleOpenCustomerViewModel GetSingleOpenCustomerInfo(GetSingleOpenCustomerInfoQuery query)
        {
            var criteria = new GetSingleOpenCustomerInfoCriteria(query.CustId);
            var affectedDownlines = dataAccess.GetSingleOpenAffectedDownlines(criteria);

            return new SingleOpenCustomerViewModel(query, affectedDownlines);
        }

        public MultipleOpenCustomerViewModel GetMultipleOpenCustomerInfo(GetMultipleOpenCustomerInfoQuery query)
        {
            var criteria = new GetMultipleOpenCustomerInfoCriteria(query.CustIdList, query.RoleId);

            return new MultipleOpenCustomerViewModel(query, dataAccess.GetCustomerClosedStatusList(criteria));
        }

        public SingleOpenDownlinesViewModel GetSingleOpenDownlinesInfo(GetSingleOpenDownlinesInfoQuery query)
        {
            var criteria = new GetSingleOpenDownlinesInfoCriteria(
                query.CustId,
                query.IsAffected,
                query.DownlineRoleId,
                query.PageSize,
                query.PageIndex);

            var data = dataAccess.GetSingleOpenDownlines(criteria);

            return new SingleOpenDownlinesViewModel(data.Item1, data.Item2);
        }

        public Result UpdateSingleCustomerClosedStatus(UpdateSingleClosedStatusQuery query)
            => dataAccess.UpdateCustomerClosedStatus(new UpdateClosedStatusCriteria(
                query.UserId,
                query.CustId,
                query.IsClosed,
                query.Language,
                query.Ip,
                query.SystemId));

        public IEnumerable<Result> UpdateMultipleCustomerClosedStatus(IEnumerable<UpdateMultipleClosedStatusQuery> query, int roleId)
        {
            var result = new List<Result>();
            var successUpdateClosedList = new StringBuilder();
            var successUpdateOpenedList = new StringBuilder();

            foreach (var item in query)
            {
                var criteria = new UpdateClosedStatusCriteria(
                    item.UserId,
                    item.CustId,
                    item.IsClosed,
                    item.Language,
                    item.Ip,
                    item.SystemId);
                var preResult = dataAccess.UpdateCustomerClosedStatus(criteria);
                if (preResult.IsSuccess)
                {
                    if (item.IsClosed)
                    {
                        successUpdateClosedList.Append(item.UserName);
                        successUpdateClosedList.Append(", ");
                    }
                    else
                    {
                        successUpdateOpenedList.Append(item.UserName);
                        successUpdateOpenedList.Append(", ");
                    }
                }
                else
                {
                    result.Add(new Result(preResult.ErrorCode, $"{item.UserName}: {preResult.ErrorMessage}"));
                }
            }

            const int indent = 2;

            if (successUpdateClosedList.Length > 0)
            {
                successUpdateClosedList.Length -= indent;
                string message = roleId != Role.Member
                    ? $"{successUpdateClosedList.ToString()} and his/her downlines have been successfully closed."
                    : $"{successUpdateClosedList.ToString()} have been successfully closed.";
                result.Insert(0, new Result(0, message));
            }

            if (successUpdateOpenedList.Length > 0)
            {
                successUpdateOpenedList.Length -= indent;
                string message = roleId != Role.Member
                    ? $"{successUpdateOpenedList.ToString()} and his/her affected downlines have been successfully opened."
                    : $"{successUpdateOpenedList.ToString()} have been successfully opened.";
                result.Insert(0, new Result(0, message));
            }

            return result;
        }

        public IEnumerable<CustomerProtectedStatus> GetMultipleProtectedStatus(string groupName, string custIds, byte roleId, int userId)
            => dataAccess.GetMultipleProtectedStatus(new GetMultipleProtectedStatusCriteria(groupName, custIds, roleId, userId));

        public IEnumerable<Result> UpdateMultipleProtectedStatus(CustomerProtectedStatus[] protectedStatus, int userId, string ip)
        {
            Result result;
            IList<Result> errorResults = new List<Result>();

            foreach (var item in protectedStatus)
            {
                result = UpdateProtectedStatus(item.CustId, item.RoleId, item.IsProtectedMode, userId, ip);

                if (!result.IsSuccess)
                {
                    result.ErrorMessage = $"{item.Username}: {result.ErrorMessage}";
                    errorResults.Add(result);
                }
            }

            return BuildResults(protectedStatus.Length, errorResults);
        }

        private static IEnumerable<Result> BuildResults(int total, IList<Result> errorResults)
        {
            var errorCount = errorResults.Count;

            if (errorCount == 0)
            {
                errorResults.Add(new Result(errMsg: ViewLang.EditMultipleAllSuccessfully));
            }
            else if (errorCount < total)
            {
                errorResults.Insert(0, new Result(errMsg: ViewLang.EditMultiplePartialSuccessfully));
            }

            return errorResults;
        }

        public Result UpdateProtectedStatus(int custId, byte roleId, bool isProtected, int userId, string ip)
        {
            var result = dataAccess.UpdateProtectedStatus(
                new UpdateProtectedStatusCriteria(custId, isProtected, userId, ip));

            if (result.IsSuccess && isProtected)
            {
                KickoutCustomer(custId, roleId, ip, "ChangeProtectionMode");
            }

            return result;
        }

        public Result CheckMultipleSupersBetSetting(string sourceUser, string destUsers, string groupName, int userId, bool isCopy = true)
            => dataAccess.CheckMultipleSupersBetSetting(new CheckMultipleSupersBetSettingCriteria(sourceUser, destUsers, groupName, isCopy));
    }
}