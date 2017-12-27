namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Core.Context;
    using Criteria;
    using DTO;
    using Extensions;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.Logging.Extensions.Web;
    using IBCUtils;
    using Repositories;
    using Security;
    using Service.Impl.Customer.Casino;
    using Utilities;
    using ViewModels;
    using System.Diagnostics;

    public class LeftPanelServiceImpl : LeftPanelService
    {
        private readonly LeftPanelDao dataAccess;

        public LeftPanelServiceImpl(LeftPanelDao dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new LeftPanelDaoImpl();
        }

        public CustomerModel GetCustomerInfo(int actorId, string username, string groupName = "ACCDDD")
        {
            var ds = dataAccess.GetCustomerInfo(
                string.IsNullOrWhiteSpace(username)
                    ? string.Empty
                    : username,
                groupName);

            if (ds == null || ds.Tables.Count <= 0)
            {
                return null;
            }

            var model = new CustomerModel();
            var tableCustomer = ds.Tables[0];
            var customerInfoRow = tableCustomer.Rows[0];
            var roleId = Utils.ToInt32(customerInfoRow["roleid"]);

            model.HasDownline = Utils.ToBoolean(customerInfoRow["HasDownline"]);
            model.UplineHasSecurityCode = Utils.ToBoolean(customerInfoRow["UplineHasSecurityCode"]);

            model.BalanceInfo(ds);
            model.PersonalInfo(ds);
            model.LoginInfo(ds);
            model.CurrencyId = customerInfoRow.ToInt32("exchangeid");

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

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
            => dataAccess.UpdateGivenCredit(id, uplineId, newGivenCredit);

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

        public OnlineList GetOnlineList(int custId)
        {
            var model = new OnlineList();
            foreach (DataRow row in dataAccess.GetOnlineList(custId))
            {
                model.Onlines.Add(new OnlineInfo
                (
                    new ObjectInfo { Text = CustomerModelExtensions.CheckVNIP(row.ToString("ip")) },
                    new ObjectInfo { Text = Utils.ToDateTime(row["reload_time"]).ToStringExt() }
                ));
            }
            return model;
        }

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
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
            }

            return result;
        }

        public Result UpdateSecurityCode(int custId, string code, bool force, int roleId)
        {
            var hashCode = Encryption.CfsEnCode(code);

            if (!roleId.IsNotMember())
            {
                return dataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
            }

            var md5 = Cryptography.ComputeHash("MD5", custId.ToString(), "1qaw3+{:P)"); //DevSkim: ignore DS126858
            hashCode = Cryptography.ComputeHash("SHA1", code, md5); //DevSkim: ignore DS126858

            return dataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
        }

        public Result EnableHrFixedOdds(int id, bool enable, bool enableDownline) =>
            dataAccess.EnableHRFixedOdds(id, enable, enableDownline);

        public Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId) =>
            dataAccess.UpdateGoldDeluxe(id, enable, enableDownline, custName, userId);

        public Result RemoveOtp(int custId) => dataAccess.RemoveOTP(new RemoveOtpCriteria
        {
            Custid = custId,
            Userid = CurrentSession.Instance.UserId,
            IP = CurrentContext.Instance.Ip4Address
        });
    }
}