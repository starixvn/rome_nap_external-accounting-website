namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System;
    using System.Diagnostics;
    using CasinoLibrary.Model.UserException;
    using CasinoLibrary.SBAgent;
    using Core.Authorization;
    using Core.Shared;
    using Criteria;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DataAccesses;
    using DTO;
    using IBCUtils;
    using Logging.Extensions.Web;
    using Security;
    using Service.Impl.Customer.Casino;
    using Utilities;

    public interface CustomerCommandService
    {
        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        Result CloseCutomer(int custId, int uplineId, ICurrentSession currentSession, ICurrentContext currentContext,
            bool close, bool clearCredit, string custName);

        Result SuspendCustomer(int custId, bool suspend);

        Result AllowOutright(int custId, bool allow);

        Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName);

        Result EnableNumberGame(int id, bool enable, bool enableDownline);

        Result EnableLiveCasino(int id, bool enable, bool enableDownline);

        Result EnableESports(int id, bool enable, bool enableDownline);

        Result AutoMasterPt(int custId, bool allow, int uplineId, int userId, string username, string ipAddress);

        Result UpdateSecurityCode(int custId, string code, bool force, int roleId);

        Result EnableHrFixedOdds(int id, bool enable, bool enableDownline);

        Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId);

        Result RemoveOtp(int custId, int userId, string ip4Address);
    }

    public class CustomerCommandServiceImpl : CustomerCommandService
    {
        private const int AlphaAdminSystem = 3;
        private const string Salt = "1qaw3+{:P)";
        private readonly AuthorizationService authorizationService;
        private readonly ICasinoManager casinoManager;

        private readonly CustomerDataAccess customerDataAccess;

        public CustomerCommandServiceImpl(
            CustomerDataAccess customerDataAccess = null,
            ICasinoManager casinoManager = null,
            AuthorizationService authorizationService = null)
        {
            this.customerDataAccess = customerDataAccess ?? new CustomerDataAccessImpl();
            this.casinoManager = casinoManager ?? new CasinoManager();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
        }

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
            => customerDataAccess.UpdateGivenCredit(id, uplineId, newGivenCredit);

        public Result ResetMemberCredit(int custId, int agentId, bool closedOnly) =>
            customerDataAccess.ResetMemberCredit(custId, agentId, closedOnly);

        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
            => customerDataAccess.CustomerInsertClearBalance(custId, custIds, isCheckedAll, ip);

        public Result CloseCutomer(int custId, int uplineId, ICurrentSession currentSession,
            ICurrentContext currentContext, bool close, bool clearCredit, string custName)
        {
            var casinoCustomer = casinoManager.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer?.RoleId == Role.Super
                && !close
                && !authorizationService.IsAuthorized(currentSession.UserId, eFunctions.OpenSuper,
                    AuthenticateAction.AuthenticationType.Host))
            {
                return new Result(1, ViewLang.DontHavePermission);
            }

            if (casinoCustomer?.CasinoSync == true)
            {
                UpdateCasinoClosed(custId, close, currentSession, currentContext);
            }

            return customerDataAccess.CloseCutomer(custId, close, clearCredit, custName);
        }

        public Result SuspendCustomer(int custId, bool suspend)
            => customerDataAccess.SuspendCustomer(custId, suspend);

        public Result AllowOutright(int custId, bool allow)
            => customerDataAccess.AllowOutright(custId, allow);

        public Result EnableCasino(int id, bool enable, bool enableDownline, bool sync, string custName)
            => customerDataAccess.EnableCasino(id, enable, enableDownline, sync, custName);

        public Result EnableLiveCasino(int id, bool enable, bool enableDownline)
            => customerDataAccess.EnableLiveCasino(id, enable, enableDownline);

        public Result EnableNumberGame(int id, bool enable, bool enableDownline)
            => customerDataAccess.EnableNumberGame(id, enable, enableDownline);

        public Result EnableESports(int id, bool enable, bool enableDownline)
            => customerDataAccess.EnableESports(id, enable, enableDownline);

        public Result AutoMasterPt(int custId, bool allow, int uplineId, int userId, string username, string ipAddress)
        {
            var result = customerDataAccess.AutoMasterPT(custId, allow);
            var custInfo = casinoManager.GetCustInfoIBC(custId, uplineId);

            if (!custInfo.CasinoSync)
            {
                return result;
            }

            var casinoSync = new Customer();

            try
            {
                casinoSync.UpdateMasterDisableAutoMPT(
                    custId,
                    allow,
                    ipAddress,
                    userId,
                    username,
                    AlphaAdminSystem);
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
                return customerDataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
            }

            var md5 = Cryptography.ComputeHash("MD5", custId.ToString(), Salt); //DevSkim: ignore DS126858
            hashCode = Cryptography.ComputeHash("SHA1", code, md5); //DevSkim: ignore DS126858

            return customerDataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
        }

        public Result EnableHrFixedOdds(int id, bool enable, bool enableDownline)
            => customerDataAccess.EnableHRFixedOdds(id, enable, enableDownline);

        public Result UpdateGoldDeluxe(int id, bool enable, bool enableDownline, string custName, int userId)
            => customerDataAccess.UpdateGoldDeluxe(id, enable, enableDownline, custName, userId);

        public Result RemoveOtp(int custId, int userId, string ip4Address)
            => customerDataAccess.RemoveOTP(new RemoveOtpCriteria
            {
                Custid = custId,
                Userid = userId,
                IP = ip4Address
            });

        protected void UpdateCasinoClosed(int custId, bool closed, ICurrentSession currentSession,
            ICurrentContext currentContext)
        {
            var cust = new Customer();
            try
            {
                if (authorizationService.IsAuthorized(
                    currentSession.UserId,
                    eFunctions.CloseCustomer,
                    AuthenticateAction.AuthenticationType.Host))
                {
                    cust.UpdateCustClosed(
                        custId,
                        closed,
                        currentContext.Ip4Address,
                        currentSession.UserId,
                        currentSession.UserName,
                        AlphaAdminSystem);
                }
            }
            catch (ErrorException casinoEx)
            {
                ExceptionHandler.Handle(new Exception(casinoEx.Message, casinoEx), TraceEventType.Information);
            }
            catch (Exception ex)
            {
                ExceptionHandler.Handle(ex);
            }
        }
    }
}