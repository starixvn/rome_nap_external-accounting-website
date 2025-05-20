namespace Fanex.ExternalAccount.DataAccess.Impl.Customer
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Common;
    using Common.Extensions;
    using Core.Shared;
    using Criteria.Customer;
    using Criteria.Customer.CustomerStatus;
    using DTO;
    using DTO.Customer;
    using DTO.Customer.Status;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.ViewModel.Customer.Suspend;
    using Interface.Customer;

    public class CustomerDao : ICustomerDao
    {
        private readonly IDynamicRepository _dynamicRepository;

        public CustomerDao() : this(null)
        {
        }

        public CustomerDao(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public Customer GetCustomerByUserName(string userName, string groupName, int userId)
            => _dynamicRepository.Fetch<Customer>(new GetCustomerByUserNameCriteria(userName, groupName, userId)).FirstOrDefault();

        public bool IsArchivedCustId(int custId)
        {
            var command = new IsArchivedCustIdCommand(custId);
            _dynamicRepository.Execute(new IsArchivedCustIdCommand(custId));
            return command.IsArchived;
        }

        public Customer UserInfo(int custId, byte settingStatus)
            => _dynamicRepository.Get<Customer>(new GetUserInfoCriteria(custId, settingStatus));

        public Result EditUserInfo(Customer customer, string ip, int userId, int subAccId, string language)
        {
            var criteria = new UpdateUserInfoCommand(
                customer.CustId,
                customer.FirstName,
                customer.LastName,
                customer.Phone,
                customer.MobilePhone,
                customer.Email,
                customer.Fax,
                ip,
                userId,
                subAccId,
                language,
                customer.Remark
            );

            _dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrorCode, criteria.ErrorMessage);
        }

        public Result ResetPassword(ResetPasswordCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result ConfigDoubleComm(UpdateDoubleCommCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public IEnumerable<SingleUpdateAffectedDownline> GetSingleUpdateSuspendAffectedDownlines(GetSingleSuspendCustomerInfoCriteria criteria)
            => _dynamicRepository.Fetch<SingleUpdateAffectedDownline>(criteria);

        public SuspendedCustomerDetailViewModel GetSuspendDownlinesDetail(GetSuspendCustomerDetailInfoCriteria criteria)
        {
            var listDownlines = new List<UnsuspendDownlineDetail>();
            var result = _dynamicRepository.Fetch<dynamic>(criteria);

            foreach (var item in result)
            {
                listDownlines.Add(new UnsuspendDownlineDetail(Convert.ToInt32(item.roleid), Convert.ToString(item.username)));
            }

            return new SuspendedCustomerDetailViewModel(criteria.TotalRows, listDownlines);
        }

        public Result SuspendCustomer(UpdateSuspendCustomerCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public List<CustomerSuspendStatusRow> GetMultipleSuspendCustomerViewModel(GetMultipleSuspendCustomerInfoCriteria criteria)
        {
            var dbResult = _dynamicRepository.FetchMultiple<CustomerSuspendStatusRow, CustomerSuspendStatusRow>(criteria);
            var result1 = dbResult.Item1.ToList();

            result1.ForEach(item => item.RoleId = criteria.RoleId);
            var result2 = dbResult.Item2.ToList();
            if (result2.Any())
            {
                foreach (var item in result2)
                {
                    var custid = item.CustId;
                    var row = result1.FirstOrDefault(m => m.CustId == custid);
                    if (row != null)
                    {
                        var roleid = item.RoleId;
                        switch (roleid)
                        {
                            case Role.Master:
                                row.AffectedMaster = item.CntAffected;
                                row.RemainUnchangedMaster = item.CntUnchanged;
                                break;
                            case Role.Agent:
                                row.AffectedAgent = item.CntAffected;
                                row.RemainUnchangedAgent = item.CntUnchanged;
                                break;
                            default:
                                row.AffectedMember = item.CntAffected;
                                row.RemainUnchangedMember = item.CntUnchanged;
                                break;
                        }
                    }
                }
            }

            return result1;
        }

        public void KickoutCustomer(KickoutCustomerCommand command)
            => _dynamicRepository.Execute(command);

        public CustomerStatusDTO GetCustomerAllStatus(GetCustomerAllStatusCriteria criteria)
        {
            var resultDataSet = _dynamicRepository.FetchDataSet(criteria);
            var result = new CustomerStatusDTO();
            if (resultDataSet.Tables[0].Rows.Count <= 0)
            {
                return result;
            }

            result.HasSecurityCode = Utils.ToBoolean(resultDataSet.Tables[0].Rows[0]["HasSecurityCode"]);
            result.UplineHasSecurityCode = Utils.ToBoolean(resultDataSet.Tables[0].Rows[0]["UplineHasSecurityCode"]);
            result.DisableTransfer = Utils.ToBoolean(resultDataSet.Tables[0].Rows[0]["DisableTransfer"]);

            foreach (DataRow row in resultDataSet.Tables[1].Rows)
            {
                if (!row.IsNull("GroupId"))
                {
                    var isHidden = Utils.ToBoolean(row["isHidden"]);
                    var groupId = Convert.ToInt32(row["GroupId"]);
                    if (!isHidden && !result.ProductGroups.Any(x => x.Id == groupId))
                    {
                        result.ProductGroups.Add(new Product(groupId, groupId, CacheRef.GetProductGroupName(groupId)));
                    }
                }
            }

            return result;
        }

        public bool CheckLimitNewDownline(CheckLimitNewDownlineCommand command)
        {
            _dynamicRepository.Execute(command);
            return command.IsAllowed;
        }

        public IEnumerable<SingleOpenAffectedDownline> GetSingleOpenAffectedDownlines(GetSingleOpenCustomerInfoCriteria criteria)
            => _dynamicRepository.Fetch<SingleOpenAffectedDownline>(criteria);

        public IEnumerable<CustomerClosedStatusRow> GetCustomerClosedStatusList(GetMultipleOpenCustomerInfoCriteria criteria)
        {
            var dbResult = _dynamicRepository.FetchMultiple<CustomerClosedStatusRow, CustomerClosedStatusRow>(criteria);
            var result1 = dbResult.Item1.ToList();

            result1.ForEach(item => item.RoleId = criteria.RoleId);
            var result2 = dbResult.Item2.ToList();
            if (result2.Any())
            {
                foreach (var item in result2)
                {
                    var custid = item.CustId;
                    var row = result1.FirstOrDefault(m => m.CustId == custid);
                    if (row != null)
                    {
                        var roleid = item.RoleId;
                        switch (roleid)
                        {
                            case Role.Master:
                                row.AffectedMaster = item.CntAffected;
                                row.RemainUnchangedMaster = item.CntUnchanged;
                                break;
                            case Role.Agent:
                                row.AffectedAgent = item.CntAffected;
                                row.RemainUnchangedAgent = item.CntUnchanged;
                                break;
                            default:
                                row.AffectedMember = item.CntAffected;
                                row.RemainUnchangedMember = item.CntUnchanged;
                                break;
                        }
                    }
                }
            }

            return result1;
        }

        public Tuple<int, List<SingleOpenDownline>> GetSingleOpenDownlines(GetSingleOpenDownlinesInfoCriteria criteria)
        {
            var singleOpenDownlines = _dynamicRepository.Fetch<SingleOpenDownline>(criteria).ToList();
            return Tuple.Create(criteria.TotalRows, singleOpenDownlines);
        }

        public Result UpdateCustomerClosedStatus(UpdateClosedStatusCriteria criteria)
        {
            _dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public IEnumerable<CustomerProtectedStatus> GetMultipleProtectedStatus(GetMultipleProtectedStatusCriteria criteria)
            => _dynamicRepository.Fetch<CustomerProtectedStatus>(criteria);

        public Result UpdateProtectedStatus(UpdateProtectedStatusCriteria criteria)
        {
            _dynamicRepository.Execute(criteria);
            return new Result(criteria.ErrCode, criteria.ErrMsg);
        }

        public Result CheckMultipleSupersBetSetting(CheckMultipleSupersBetSettingCriteria criteria)
        {
            if (!criteria.IsValid())
            {
                return new Result(1, "Please input all required fields.");
            }

            var result = new Result();
            result.ResultData = string.Join(",", _dynamicRepository.Fetch<int>(criteria));
            result.ErrorMessage = criteria.ErrMsg;
            result.ErrorCode = criteria.ErrCode.ToString();
            return result;
        }
    }
}