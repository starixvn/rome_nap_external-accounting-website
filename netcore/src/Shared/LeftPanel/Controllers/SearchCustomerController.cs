namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
    using Microsoft.AspNetCore.Mvc;
    using Services;
    using ViewModels;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class SearchCustomerController : BaseController
    {
        private const int DefaultPageSize = 100;
        private const string MasterAgentCreditViewl =
            "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MasterAgentCredit.cshtml";

        private const string MemberCreditView =
            "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MemberCredit.cshtml";

        private const string SecurityCodeIndexView =
            "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml";

        private const string LeftPanelIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml";
        private const string LeftpanelBalanceIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml";
        private const string LeftPanelOtpIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/OTP/Index.cshtml";

        private readonly CustomerQueryService customerQueryService;

        public SearchCustomerController(
            CustomerQueryService customerQueryService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest,
            IConfiguration configuration,
            AuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.customerQueryService = customerQueryService;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult Index(string groupName, string custName)
        {
            var userId = CurrentSession.UserId;
            ViewBag.UserId = userId;
            ViewBag.Code = Encryptor.RandomCaptcha;
            var result = new Result
            {
                ErrorCode = string.Empty,
                ErrorMessage = string.Empty
            };

            if (string.IsNullOrWhiteSpace(custName))
            {
                return View(LeftPanelIndexView);
            }

            var customerInfo = customerQueryService.SearchCustomer(
                userId,
                custName,
                ExtRequest.GetGroupName(groupName),
                Config.SettingStatus, result);

            ViewBag.ErrorMessage = result.ErrorMessage;

            return View(LeftPanelIndexView, customerInfo);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult UsedCreditPage(int custId, int roleId)
        {
            ViewBag.CustId = custId;

            if (roleId.IsSuper() || roleId.IsMaster())
            {
                var items = customerQueryService.MasterAgentUsedCreditItems(custId, roleId);
                var directMembers = customerQueryService.MemberUsedCreditItems(custId, 0);
                return View(
                    MasterAgentCreditViewl,
                    new UserCreditModel
                    {
                        MasterAgentUsedCredit = new MasterAgentUsedCreditModel
                        {
                            MasterAgentUsedCreditItems = items,
                        },
                        DirectMemberUsedCredit = new MemberUsedCreditModel
                        {
                            MemberUsedCreditItems = directMembers
                        },
                        RoleId = roleId
                    });
            }
            else
            {
                var items = customerQueryService.MemberUsedCreditItems(custId, 0);
                var cashMembers = customerQueryService.MemberUsedCreditItems(custId, 1);
                return View(
                    MemberCreditView,
                    new MemberUsedCreditModel
                    {
                        MemberUsedCreditItems = items,
                        CashMemberUsedCreditItems = cashMembers
                    });
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult SelectClearBalance(int custId)
        {
            ViewBag.CustId = custId;
            var result = customerQueryService.CustomerSelectClearBalance(custId);
            return View(LeftpanelBalanceIndexView, result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult GetSecurityCode(int custId, int roleId, string username)
        {
            ViewBag.custId = custId;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;

            return View(SecurityCodeIndexView);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetOnlineList(int custId)
            => JsonBase(customerQueryService.GetOnlineList(custId));

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult GetDownlineList(bool isDirectDownline, int custId, int pageIndex)
            => JsonBase(customerQueryService.GetDownlineList(isDirectDownline, custId, DefaultPageSize, pageIndex));

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public IActionResult RemoveOTP(int custId, string username) =>
            View(LeftPanelOtpIndexView, new { CustId = custId, Username = username });
    }
}