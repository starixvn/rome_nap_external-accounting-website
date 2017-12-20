namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System.Collections.Generic;
    using System.Data;
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Core.Context;
    using Criteria;
    using DTO;
    using Extensions;
    using Repositories;
    using Service.Impl.Customer.Casino;
    using Shared;
    using Utilities;
    using ViewModels;

    public class LeftPanelServiceImpl : LeftPanelService
    {
        private readonly LeftPanelDao dataAccess;

        public LeftPanelServiceImpl(LeftPanelDao dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new LeftPanelDaoImpl();
        }

        public CustomerModel GetCustomerInfo(int actorId, string username, string groupName = "ACCDDD", int pageIndex = 1)
        {
            var ds = dataAccess.GetCustomerInfo(username, groupName);

            if (ds == null || ds.Tables.Count <= 0)
            {
                return null;
            }

            var model = new CustomerModel();

            var tableCustomer = ds.Tables[0];
            var roleId = Utils.ToInt32(tableCustomer.Rows[0]["roleid"]);

            model.HasDownline = Utils.ToBoolean(tableCustomer.Rows[0]["HasDownline"]);
            model.UplineHasSecurityCode = Utils.ToBoolean(tableCustomer.Rows[0]["UplineHasSecurityCode"]);

            model.BalanceInfo(ds);
            model.PersonalInfo(ds);
            model.LoginInfo(ds);
            model.CurrencyId = tableCustomer.Rows[0].ToInt32("exchangeid");

            if (roleId.IsSuper())
            {
                model.SuperInfo(ds, actorId);
            }
            else if (roleId.IsMaster())
            {
                model.MasterInfo(ds);
            }
            else if (roleId.IsAgent())
            {
                model.AgentInfo(ds);
            }
            else
            {
                model.MemberInfo(ds);
            }

            return model;
        }

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit) =>
            dataAccess.UpdateGivenCredit(id, uplineId, newGivenCredit);

        public DataTable GetDownlineList(int custid, int pageSize, int pageIndex, out int totalPages)
        {
            totalPages = 1;
            var dr = dataAccess.GetDownlineList(custid, pageSize, pageIndex);

            if (dr?.ResultSet == null || dr.ResultSet.Tables.Count <= 0)
            {
                return null;
            }

            totalPages = Utils.ToInt32(dr.Outputs["totalpages"]);

            return dr.ResultSet.Tables[0];
        }

        public DataRowCollection GetOnlineList(int custid) => dataAccess.GetOnlineList(custid);

        public IList<MasterAgentUsedCreditItem> MasterAgentUsedCreditItems(int custId, int roleId) =>
            dataAccess.MasterAgentUsedCreditItems(custId, roleId);

        public IList<MemberUsedCreditItem> MemberUsedCreditItems(int custId) =>
            dataAccess.MemberUsedCreditItems(custId);

        public Result ResetMemberCredit(int custId, int agentId, bool closedOnly) =>
            dataAccess.ResetMemberCredit(custId, agentId, closedOnly);

        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
            => dataAccess.CustomerInsertClearBalance(custId, custIds, isCheckedAll, ip);

        public DataTable CustomerSelectClearBalance(int custId) => dataAccess.CustomerSelectClearBalance(custId);

        public Result CloseCutomer(int custId, bool close, bool clearCredit, string custName)
            => dataAccess.CloseCutomer(custId, close, clearCredit, custName);

        public Result SuspendCustomer(int custId, bool suspend)
            => dataAccess.SuspendCustomer(custId, suspend);

        public Result AllowOutright(int custId, bool allow)
            => dataAccess.AllowOutright(custId, allow);

        public Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName)
            => dataAccess.EnableCasino(id, enable, enableDownline, sync, custName);

        public Result EnableLiveCasino(int id, bool enable, bool enableDownline) =>
            dataAccess.EnableLiveCasino(id, enable, enableDownline);

        public Result EnableNumberGame(int id, bool enable, bool enableDownline) =>
            dataAccess.EnableNumberGame(id, enable, enableDownline);

        public Result EnableESports(int id, bool enable, bool enableDownline) =>
            dataAccess.EnableESports(id, enable, enableDownline);

        public Result AutoMasterPt(int id, bool allow, int uplineId)
        {
            var result = dataAccess.AutoMasterPT(id, allow);

            var casinoManager = new CasinoManager();
            var custInfo = casinoManager.GetCustInfoIBC(id, uplineId);

            if (!custInfo.CasinoSync)
            {
                return result;
            }

            var casinoSync = new Customer();

            try
            {
                casinoSync.UpdateMasterDisableAutoMPT(
                    id,
                    allow,
                    CurrentContext.Instance.Ip4Address,
                    CurrentSession.Instance.UserId,
                    CurrentSession.Instance.UserName,
                    3);
            }
            catch (ErrorException casinoEx)
            {
                Logs.CasinoError(casinoEx);
            }

            return result;
        }

        public Result ResetSecurityCode(int id, string code, bool force) =>
            dataAccess.ResetSecurityCode(id, code, force);

        public Result EnableHrFixedOdds(int id, bool enable, bool enableDownline) =>
            dataAccess.EnableHRFixedOdds(id, enable, enableDownline);

        public Result
            UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId) =>
            dataAccess.UpdateGoldDeluxe(id, enable, enableDownline, custName, userId);

        public Result RemoveOtp(int custId) => dataAccess.RemoveOTP(new RemoveOtpCriteria
        {
            Custid = custId,
            Userid = CurrentSession.Instance.UserId,
            IP = CurrentContext.Instance.Ip4Address
        });
    }
}