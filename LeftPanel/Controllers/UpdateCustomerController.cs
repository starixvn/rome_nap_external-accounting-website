namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System.Threading.Tasks;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Service.Impl.Customer;
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Services;

    public class UpdateCustomerController : BaseController
    {
        private readonly CustomerCommandService customerCommandService;
        private readonly ICustomerService customerService;

        public UpdateCustomerController()
            : this(new CustomerCommandServiceImpl(), new CustomerService(), null, null, null)
        {
        }

        internal UpdateCustomerController(
            CustomerCommandService customerCommandService,
            ICustomerService customerService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest) : base(currentSession, currentContext, extRequest)
        {
            this.customerCommandService = customerCommandService;
            this.customerService = customerService;
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateCredit,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGivenCreditPost(int custId, int uplineId, decimal newGivenCredit)
            => JsonBase(customerCommandService.UpdateGivenCredit(custId, uplineId, newGivenCredit));

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetMemberCredit(int uplineId, bool closedOnly)
            => JsonBase(customerCommandService.ResetMemberCredit(CurrentSession.UserId, uplineId, closedOnly));

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
            => JsonBase(customerCommandService.CustomerInsertClearBalance(custId, custIds, isCheckedAll,
                CurrentContext.Ip4Address));

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
            => JsonBase(
                customerCommandService.CloseCutomer(
                    custId,
                    uplineId,
                    CurrentSession,
                    CurrentContext,
                    close,
                    clearCredit,
                    custName));

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.OpenSuper,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UnlockAccess(int custId, string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new System.ArgumentNullException(nameof(groupName));
            }

            return JsonBase(customerCommandService.UnlockAccess(custId, CurrentSession.UserId, CurrentContext.Ip4Address));
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.ProtectCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateProtectedStatus(int custId, byte roleId, bool isProtected, string groupName)
        {
            if (string.IsNullOrWhiteSpace(groupName))
            {
                throw new System.ArgumentNullException(nameof(groupName));
            }

            return JsonBase(customerService.UpdateProtectedStatus(custId, roleId, isProtected, CurrentSession.UserId, CurrentContext.Ip4Address));
        }


        [ExternalAccountAuthenticate(Function = eFunctions.AllowOutright,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateOutright(int custId, bool allow)
            => JsonBase(customerCommandService.AllowOutright(custId, allow));

        [ExternalAccountAuthenticate(Function = eFunctions.AllowAutoMPT,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateAutoMPT(int custId, bool allow, int uplineId)
            => JsonBase(customerCommandService.AutoMasterPt(custId, allow, uplineId, CurrentSession.UserId,
                CurrentSession.UserName, CurrentContext.Ip4Address));

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSecurityCode(int custId, string code, bool force, int roleId = 2)
            => JsonBase(customerCommandService.UpdateSecurityCode(custId, code, force, roleId));

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public async Task<ActionResult> RemoveOTP(int custId)
            => JsonBase(await customerCommandService.RemoveOtp(custId, CurrentSession.UserId, CurrentContext.Ip4Address));
    }
}