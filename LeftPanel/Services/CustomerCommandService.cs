namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Data;
    using System.Diagnostics;
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Core.Context;
    using Core.Shared;
    using Criteria;
    using DTO;
    using IBCUtils;
    using Logging.Extensions.Web;
    using Repositories;
    using Security;
    using Service.Impl.Customer.Casino;

    public interface CustomerCommandService
    {
        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        DataTable CustomerSelectClearBalance(int custId);

        Result CloseCutomer(int custId, bool close, bool clearCredit, string custName);

        Result SuspendCustomer(int custId, bool suspend);

        Result AllowOutright(int custId, bool allow);

        Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName);

        Result EnableNumberGame(int id, bool enable, bool enableDownline);

        Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        Result EnableESports(int id, bool enable, bool enableDownline);

        Result AutoMasterPt(int id, bool allow, int uplineId);

        Result UpdateSecurityCode(int custId, string code, bool force, int roleId);

        Result EnableHrFixedOdds(int id, bool enable, bool enableDownline);

        Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId);

        Result RemoveOtp(int custId);
    }

    public class CustomerCommandServiceImpl : CustomerCommandService
    {
        private readonly LeftPanelDao dataAccess;

        public CustomerCommandServiceImpl(LeftPanelDao dataAccess = null)
        {
            this.dataAccess = dataAccess ?? new LeftPanelDaoImpl();
        }

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
           => dataAccess.UpdateGivenCredit(id, uplineId, newGivenCredit);

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