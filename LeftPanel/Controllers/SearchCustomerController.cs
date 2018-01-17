namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Service.Impl.Customer.Casino;
    using Services;
    using ViewModels;

    public class SearchCustomerController : BaseController
    {
        private const string MasterAgentCreditViewl =
            "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MasterAgentCredit.cshtml";

        private const string MemberCreditView =
            "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MemberCredit.cshtml";

        private const string SecurityCodeIndexView =
            "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml";

        private const string LeftPanelIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml";
        private const string LeftpanelBalanceIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml";
        private const string LeftPanelOtpIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/OTP/Index.cshtml";
        private readonly CustomerCommandService customerCommandService = new CustomerCommandServiceImpl();

        private readonly CustomerQueryService customerQueryService = new CustomerQueryServiceImpl();

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult Index(string custName = null, string uplineId = "ACCDDD")
        {
            var userId = CurrentSession.UserId;
            ViewBag.UserId = userId;

            if (string.IsNullOrWhiteSpace(custName))
            {
                return View(LeftPanelIndexView);
            }

            var checkingResult = CheckCustomerBelongToSpecialSite(custName);
            if (!IsAdminHasPermissionOnCustomer(checkingResult.IsSpecialSite
                                                || checkingResult.IsSpecialHeadline))
            {
                return RedirectToAction("NoPermission", "Shared");
            }

            var customerInfo = customerQueryService.SearchCustomer(
                userId,
                custName,
                uplineId);

            return View(LeftPanelIndexView, customerInfo);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult UsedCreditPage(int custId, int roleId)
        {
            ViewBag.CustId = custId;

            if (roleId.IsSuper() || roleId.IsMaster())
            {
                var items = customerQueryService.MasterAgentUsedCreditItems(custId, roleId);

                return View(
                    MasterAgentCreditViewl,
                    new MasterAgentUsedCreditModel
                    {
                        MasterAgentUsedCreditItems = items
                    });
            }
            else
            {
                var items = customerQueryService.MemberUsedCreditItems(custId);

                return View(
                    MemberCreditView,
                    new MemberUsedCreditModel
                    {
                        MemberUsedCreditItems = items
                    });
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult SelectClearBalance(int custId)
        {
            ViewBag.CustId = custId;
            var result = customerCommandService.CustomerSelectClearBalance(custId);
            return View(LeftpanelBalanceIndexView, result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableCasino,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return JsonBase(customerCommandService.EnableCasino(custId, enable, enableDownline,
                    casinoCustomer.CasinoSync,
                    custName));
            }
            catch (Exception ex)
            {
                return JsonBase(new
                {
                    IsSuccess = ex.Message.Contains("Not casino"),
                    ErrorCode = 1,
                    ErrorMessage = ex.Message
                });
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult GetSecurityCode(int custId, int roleId, string username)
        {
            ViewBag.custId = custId;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;

            return View(SecurityCodeIndexView);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetOnlineList(int custId)
            => JsonBase(customerQueryService.GetOnlineList(custId));

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetDownlineList(int custId, int pageIndex)
            => JsonBase(customerQueryService.GetDownlineList(custId, 100, pageIndex));

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult RemoveOTP(int custId, string username) =>
            View(LeftPanelOtpIndexView, new {CustId = custId, Username = username});
    }
}