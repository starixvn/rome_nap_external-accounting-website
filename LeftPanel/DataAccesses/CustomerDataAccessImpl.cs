namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using System.Collections.Generic;
    using System.Data;
    using System.Web;
    using Core.Context;
    using Core.Shared;
    using Criteria;
    using DTO;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.Module.LeftPanel.Models;
    using Utilities;
    using ViewModels;

    public class CustomerDataAccessImpl : CustomerDataAccess
    {
        private readonly IDynamicRepository _dynamicRepository;

        public CustomerDataAccessImpl() : this(null)
        {
        }

        public CustomerDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public DataSet GetPersonalInfo(GetPersonalInfoCriteria criteria)
            => _dynamicRepository.FetchDataSet(criteria);

        public DataSet GetLoginInfo(int custId, string groupName)
            => _dynamicRepository.FetchDataSet(new GetLogginInfoCriteria(custId, groupName));

        public DataSet GetCustomerStatusInfo(int custId, byte settingStatus)
            => _dynamicRepository.FetchDataSet(new GetCustomerStatusInfoCriteria(custId, settingStatus));

        public DataSet GetBalanceInfo(int custId)
            => _dynamicRepository.FetchDataSet(new GetBalanceInfoCriteria(custId));

        public Result UpdateMemberGivenCredit(int id, int uplineId, decimal newGivenCredit, decimal totalAmt)
        {
            var command = new UpdateMemberGivenCreditCommand(id, uplineId, newGivenCredit, totalAmt, CurrentContext.Instance.Ip4Address);
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result UpdateSMAGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            var command = new UpdateSMAGivenCreditCommand(id, uplineId, newGivenCredit, CurrentContext.Instance.Ip4Address);
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public DataSet GetDownlineList(bool isDirectDownline, int custid, int pageSize, int pageIndex, out int totalPages)
        {
            var criteria = new GetDownlineListCriteria(custid, pageSize, pageIndex, 0);
            totalPages = criteria.TotalPages;
            return _dynamicRepository.FetchDataSet(criteria);
        }

        public DataSet GetOnlineList(int custid)
            => _dynamicRepository.FetchDataSet(new GetOnlineListCriteria(custid));

        public IEnumerable<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int role)
        {
            if (role.IsSuper())
            {
                return MasterUsedCreditItems(custId);
            }

            return role.IsMaster() ? AgentUsedCreditItems(custId) : null;
        }

        public IEnumerable<MemberUsedCreditItem> MemberUsedCreditItems(int custId)
            => _dynamicRepository.Fetch<MemberUsedCreditItem>(new GetMemberUsedCreditItemsCriteria(custId));

        public Result ResetMemberCredit(int custId, int uplineId, bool closedOnly)
        {
            var command = new ResetMemberCreditCommand(custId, agentId, closedOnly, HttpContext.Current.Request.UserHostAddress);
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip, SiteDB siteDB)
        {
            var command = new CustomerInsertClearBalanceCommand(
                custId,
                custIds,
                CurrentSession.Instance.UserId,
                "E",
                !isCheckedAll ? string.Empty : "All",
                ip,
                "EXTERNAL",
                siteDB
            );
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public DataTable CustomerSelectClearBalance(int custId)
        {
            var criteria = new GetCustomerSelectClearBalanceCriteria(custId);
            var dataSet = _dynamicRepository.FetchDataSet(criteria);

            return dataSet?.Tables?[0];
        }

        public decimal GetTotalAmount(int custId, SiteDB siteDB)
        {
            var criteria = new GetTotalAmountCriteria(custId, siteDB);
            var dataSet = _dynamicRepository.FetchDataSet(criteria);

            return dataSet != null && dataSet.Tables.Count > 0 && dataSet.Tables[0].Rows.Count > 0
                ? Utils.ToDecimal(dataSet.Tables[0].Rows[0]["totalamt"])
                : 0;
        }

        public Result CloseCustomer(int custId, bool closed, bool clearCredit, string custName)
        {
            var command = new CloseCustomerCommand(
                custId,
                closed,
                clearCredit,
                custName,
                CurrentSession.Instance.UserId,
                "E",
                CurrentContext.Instance.Ip4Address,
                Config.SystemId
            );
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result UnlockAccess(UnlockAccessCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result AllowOutright(int custId, bool allow)
        {
            var command = new AllowOutrightCommand(
                custId,
                !allow,
                CurrentSession.Instance.UserId,
                "E",
                CurrentContext.Instance.Ip4Address
            );
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result AutoMasterPT(int id, bool allow)
        {
            var command = new AutoMasterPTCommand(
                id,
                !allow,
                CurrentSession.Instance.UserId,
                "E",
                CurrentContext.Instance.Ip4Address
            );
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result ResetSecurityCode(int id, string code, bool force)
        {
            var command = new ResetSecurityCodeCommand(
                id,
                CurrentSession.Instance.UserId,
                force,
                code,
                "En",
                CurrentContext.Instance.Ip4Address
            );
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result RemoveOTP(RemoveOtpCommand command)
        {
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        private IEnumerable<MasterAgentUsedCreditItem> MasterUsedCreditItems(int custId)
            => _dynamicRepository.Fetch<MasterAgentUsedCreditItem>(new GetMasterUsedCreditItemsCriteria(custId));

        private IEnumerable<MasterAgentUsedCreditItem> AgentUsedCreditItems(int custId)
            => _dynamicRepository.Fetch<MasterAgentUsedCreditItem>(new GetAgentUsedCreditItemsCriteria(custId));

        public Result ClearMemberCredit(decimal totalAmt, string username = null)
        {
            var command = new ClearMemberCreditCommand(totalAmt, username, CurrentSession.Instance.UserId, CurrentContext.Instance.Ip4Address);
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public Result ClearSMACredit(string username = null)
        {
            var command = new ClearSMACreditCommand(username, CurrentSession.Instance.UserId, CurrentContext.Instance.Ip4Address);
            _dynamicRepository.Execute(command);
            return new Result(command.ErrCode, command.ErrMsg);
        }

        public MemberBalanceInfoModel GetMemberBalanceModel(int custId, SiteDB siteDB)
        {
            var command = new GetMemberBalanceModelCommand(custId, siteDB);
            _dynamicRepository.Execute(command);

            return new MemberBalanceInfoModel(command.Balance, command.PendingTransfer3rd, command.NetPosition);
        }

        public decimal GetMemberOustanding(int custId)
        {
            var command = new GetMemberOutstandingCommand(custId);
            _dynamicRepository.Execute(command);

            return command.Outstanding;
        }
    }
}