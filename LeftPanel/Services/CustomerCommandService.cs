namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using Core.Authorization;
    using Core.Shared;
    using Criteria;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DataAccesses;
    using DTO;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Sunplus.Agent.Utilities;
    using Utilities;
    using Encryptor = Sunplus.Agent.Utilities.Encryptor;

    public interface CustomerCommandService
    {
        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        Result ResetMemberCredit(int custId, int agentId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        Result CloseCutomer(int custId, int uplineId, ICurrentSession currentSession, ICurrentContext currentContext,
            bool close, bool clearCredit, string custName);

        Result UnlockAccess(int custId, int userId, string ip4Address);

        Result AllowOutright(int custId, bool allow);

        Result AutoMasterPt(int custId, bool allow, int uplineId, int userId, string username, string ipAddress);

        Result UpdateSecurityCode(int custId, string code, bool force, int roleId);

        Result RemoveOtp(int custId, int userId, string ip4Address);
    }

    public class CustomerCommandServiceImpl : CustomerCommandService
    {
        private const int AlphaAdminSystem = 3;
        private const string Salt = "1qaw3+{:P)";
        private readonly AuthorizationService authorizationService;

        private readonly CustomerDataAccess customerDataAccess;
        private readonly Encryptor encryptor;
        private readonly IReportService reportService;

        public CustomerCommandServiceImpl(
            CustomerDataAccess customerDataAccess = null,
            AuthorizationService authorizationService = null,
            IReportService reportService = null,
            Encryptor encryptor = null)
        {
            this.customerDataAccess = customerDataAccess ?? new CustomerDataAccessImpl();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.encryptor = encryptor ?? new EncryptorImpl();
            this.reportService = reportService ?? new ReportService();
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
            var custInfo = reportService.SearchCustomerInfo(ExtRequest.Instance.GetGroupName(), custId, custName, true);
            if (custInfo.RoleId == Role.Super
                && !close
                && !authorizationService.IsAuthorized(currentSession.UserId, eFunctions.OpenSuper,
                    AuthenticateAction.AuthenticationType.Host))
            {
                return new Result(1, ViewLang.DontHavePermission);
            }

            return customerDataAccess.CloseCutomer(custId, close, clearCredit, custName);
        }

        public Result UnlockAccess(int custId, int userId, string ip4Address)
            => customerDataAccess.UnlockAccess(new UnlockAccessCriteria(custId, userId, 0, ip4Address, (byte)Config.SystemId));

        public Result AllowOutright(int custId, bool allow)
            => customerDataAccess.AllowOutright(custId, allow);

        public Result AutoMasterPt(int custId, bool allow, int uplineId, int userId, string username, string ipAddress) => customerDataAccess.AutoMasterPT(custId, allow);

        public Result UpdateSecurityCode(int custId, string code, bool force, int roleId)
        {
            var hashCode = encryptor.CfsEnCode(code);

            if (!roleId.IsNotMember())
            {
                return customerDataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
            }

            var md5 = encryptor.ComputeHash("MD5", custId.ToString(), Salt); //DevSkim: ignore DS126858
            hashCode = encryptor.ComputeHash("SHA1", code, md5); //DevSkim: ignore DS126858

            return customerDataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());
        }

        public Result RemoveOtp(int custId, int userId, string ip4Address)
            => customerDataAccess.RemoveOTP(new RemoveOtpCriteria
            {
                Custid = custId,
                Userid = userId,
                IP = ip4Address
            });
    }
}