namespace Fanex.ExternalAccount.Module.LeftPanel.DataAccesses
{
    using System;
    using System.Collections;
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
    using FanexCommonConverter = Fanex.Common.Converter;

#pragma warning disable S1192 // String literals should not be duplicated
    public class CustomerDataAccessImpl : CustomerDataAccess
    {
        private readonly IDynamicRepository _dynamicRepository;

        public CustomerDataAccessImpl(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public DataSet GetPersonalInfo(string username, string groupName, int userId)
            => _dynamicRepository.FetchDataSet(new GetPersonalInfoCriteria(username, groupName, userId));

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
            var hash = new Hashtable
            {
                {"username", username},
                {"userid", CurrentSession.Instance.UserId},
                {"returnerrcode", 1},
                {"errcode", 0},
                {"errmsg", string.Empty},
                {"ip", CurrentContext.Instance.Ip4Address},
                {"system", "EXTERNAL"},
                {"totalamt", totalAmt}
            };

            return ClearCredit(hash);
        }

        public Result ClearSMACredit(string username = null)
        {
            var hash = new Hashtable
            {
                {"username", username},
                {"userid", CurrentSession.Instance.UserId},
                {"returnerrcode", 1},
                {"errcode", 0},
                {"errmsg", string.Empty},
                {"ip", CurrentContext.Instance.Ip4Address},
                {"system", "EXTERNAL"}
            };
            return ClearCredit(hash);
        }

        private static Result ClearCredit(Hashtable hash)
        {
            using (var objDb = new ObjectDb("Acc_CustomerUpdClearCredit", 600))
            {
                objDb.AssignParamValues(hash);
                objDb.ExecuteNonQuery();

                return new Result(objDb.GetParameterValue("errcode"), objDb.GetParameterValue("errmsg"));
            }
        }

        public DataTable EAS_CustomerSelUserInfo(string groupname, int custId, string userName, bool isGetUplineInfo)
        {
            using (var db = new ObjectDb("EAS_CustomerSelUserInfo"))
            {
                if (custId != 0)
                {
                    db.AddInParameter("custid", DbType.Int32, custId);
                }

                db.AddInParameter("groupname", DbType.String, groupname);
                db.AddInParameter("username", DbType.String, userName);
                db.AddInParameter("isgetuplineinfo", DbType.Boolean, isGetUplineInfo);

                var ds = db.ExecuteDataSet();

                return ds?.Tables.Count > 0 ? ds.Tables[0] : null;
            }
        }

        public MemberBalanceInfoModel GetMemberBalanceModel(int custId, SiteDB siteDB)
        {
            var prefixSp = siteDB.IsSiteDB ? $"SiteDB{siteDB.SiteDBGroupID}_" : string.Empty;
            using (ObjectDb objdb = new ObjectDb($"{prefixSp}Acc_BalanceMemberSel", 180))
            {
                var param = new Hashtable
                {
                    { "custid", custId }
                };

                objdb.ExecuteNonQueryScaleoutDB(param, siteDB.AGIP);
                return new MemberBalanceInfoModel(
                    FanexCommonConverter.ToDecimal(objdb.GetParameterValue("balance")),
                    FanexCommonConverter.ToDecimal(objdb.GetParameterValue("PendingTransfer3rd")),
                    FanexCommonConverter.ToDecimal(objdb.GetParameterValue("netposition")));
            }
        }

        public decimal GetMemberOustanding(int custId)
        {
            using (ObjectDb objdb = new ObjectDb("Acc_OutstandingMemberSel", 180))
            {
                var param = new Hashtable
                {
                    { "custid", custId }
                };

                objdb.AssignParamValues(param);
                objdb.ExecuteNonQuery();
                return FanexCommonConverter.ToDecimal(objdb.GetParameterValue("outstanding"));
            }
        }
    }
#pragma warning restore S1192 // String literals should not be duplicated
}