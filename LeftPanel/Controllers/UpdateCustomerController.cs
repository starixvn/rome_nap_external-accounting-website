namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using Service.Impl.Customer.Casino;
    using Services;

    public class UpdateCustomerController : BaseController
    {
        private readonly CustomerCommandService customerCommandService;

        public UpdateCustomerController() : this(new CustomerCommandServiceImpl(), null, null, null)
        {
        }

        internal UpdateCustomerController(
            CustomerCommandService customerCommandService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest) : base(currentSession, currentContext, extRequest)
        {
            this.customerCommandService = customerCommandService;
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
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
            => JsonBase(customerCommandService.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly));

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

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer?.CasinoSync == true)
            {
                UpdateCasinoSuspended(custId, suspend);
            }

            return JsonBase(customerCommandService.SuspendCustomer(custId, suspend));
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
        public ActionResult RemoveOTP(int custId)
            => JsonBase(customerCommandService.RemoveOtp(custId, CurrentSession.UserId, CurrentContext.Ip4Address));
    }
}