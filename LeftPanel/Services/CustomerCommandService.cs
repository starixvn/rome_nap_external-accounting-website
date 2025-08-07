namespace Fanex.ExternalAccount.Module.LeftPanel.Services
{
    using System.Threading.Tasks;
    using Core.Authorization;
    using Core.Shared;
    using Criteria;
    using CustomerSetting.Security.Authenticate;
    using DataAccesses;
    using DTO;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.TelegramBot;
    using Sunplus.Agent.Utilities;
    using Utilities;
    using Encryptor = Sunplus.Agent.Utilities.Encryptor;

    public interface CustomerCommandService
    {
        Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit);

        Result ResetMemberCredit(int userId, int uplineId, bool closedOnly);

        Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip);

        Result CloseCutomer(int custId, int uplineId, ICurrentSession currentSession, ICurrentContext currentContext,
            bool close, bool clearCredit, string custName);

        Result UnlockAccess(int custId, int userId, string ip4Address);

        Result AllowOutright(int custId, bool allow);

        Result AutoMasterPt(int custId, bool allow, int uplineId, int userId, string username, string ipAddress);

        Result UpdateSecurityCode(int custId, string code, bool force, int roleId);

        Task<Result> RemoveOtp(int custId, int userId, string ip4Address);
    }

    public class CustomerCommandServiceImpl : CustomerCommandService
    {
        private const int AlphaAdminSystem = 3;
        private const string Salt = "1qaw3+{:P)";
        private readonly AuthorizationService authorizationService;
        private readonly ITelegramBotService telegramBotService;

        private readonly CustomerDataAccess customerDataAccess;
        private readonly Encryptor encryptor;
        private readonly IReportService reportService;
        private readonly ISiteDBConfigService siteDBService;

        public CustomerCommandServiceImpl(
            CustomerDataAccess customerDataAccess = null,
            AuthorizationService authorizationService = null,
            IReportService reportService = null,
            Encryptor encryptor = null,
            ITelegramBotService telegramBotService = null,
            ISiteDBConfigService siteDBService = null)
        {
            this.customerDataAccess = customerDataAccess ?? new CustomerDataAccessImpl();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
            this.reportService = reportService ?? new ReportService();
            this.encryptor = encryptor ?? new EncryptorImpl();
            this.telegramBotService = telegramBotService ?? new TelegramBotService();
            this.siteDBService = siteDBService ?? new SiteDBConfigService();
        }

        public Result UpdateGivenCredit(int id, int uplineId, decimal newGivenCredit)
        {
            var custInfo = reportService.SearchCustomerInfo(ExtRequest.Instance.GetGroupName(), id, string.Empty, false);
            var siteDB = siteDBService.GetSiteDBConfigByCustId(id);

            if (custInfo.RoleId == Role.Member)
            {
                var totalAmt = customerDataAccess.GetTotalAmount(id, siteDB);
                var accumCreditAllCashMember = customerDataAccess.GetCashAccumCredit(uplineId);

                return customerDataAccess.UpdateMemberGivenCredit(id, uplineId, newGivenCredit, totalAmt, accumCreditAllCashMember);
            }

            if (custInfo.RoleId == Role.Agent)
            {
                var accumCreditAllCashMember = customerDataAccess.GetCashAccumCredit(id);

                return customerDataAccess.UpdateAgentGivenCredit(id, uplineId, newGivenCredit, accumCreditAllCashMember);
            }

            // Super and Master levels (SMA)
            return customerDataAccess.UpdateSMAGivenCredit(id, uplineId, newGivenCredit);
        }


        public Result ResetMemberCredit(int userId, int uplineId, bool closedOnly) =>
            customerDataAccess.ResetMemberCredit(userId, uplineId, closedOnly);

        public Result CustomerInsertClearBalance(int custId, string custIds, bool isCheckedAll, string ip)
        {
            var siteDB = siteDBService.GetSiteDBConfigByCustId(custId);

            return customerDataAccess.CustomerInsertClearBalance(custId, custIds, isCheckedAll, ip, siteDB);
        }


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

            var result = customerDataAccess.CloseCustomer(custId, close, clearCredit, custName);
            if (result.IsSuccess && clearCredit)
            {
                if (custInfo.RoleId == Role.Member)
                {
                    var siteDB = siteDBService.GetSiteDBConfigByCustId(custId);
                    var totalAmt = customerDataAccess.GetTotalAmount(custId, siteDB);

                    return customerDataAccess.ClearMemberCredit(totalAmt, custName);
                }

                return customerDataAccess.ClearSMACredit(custName);
            }

            return result;
        }

        public Result UnlockAccess(int custId, int userId, string ip4Address)
            => customerDataAccess.UnlockAccess(new UnlockAccessCommand(custId, userId, 0, ip4Address, (byte)Config.SystemId));

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

            var result = customerDataAccess.ResetSecurityCode(custId, hashCode, force || roleId.IsNotSuper());

            if (result != null && result.IsSuccess && roleId.IsNotMember())
            {
                Task.Run(() => telegramBotService.NotifyChangeSecurityCode(custId, 0));
            }

            return result;
        }

        public async Task<Result> RemoveOtp(int custId, int userId, string ip4Address)
        {
            var result = customerDataAccess.RemoveOTP(new RemoveOtpCommand(custId, userId, ip4Address));

            var messenger = await telegramBotService.GetMessenger(custId, 0);
            if (telegramBotService.IsEnabledOTPViaTelegram(messenger))
            {
                var disableResult = await telegramBotService.DisableSendOTPViaTelegram(messenger.UserId, messenger.BotId, userId, ip4Address);
                return new Result { ErrorCode = disableResult.Code.ToString(), ErrorMessage = disableResult.Message };
            }

            return result;
        }
    }
}