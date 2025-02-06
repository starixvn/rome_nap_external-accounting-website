namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Utilities;
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

        private readonly CustomerQueryService customerQueryService;

        public SearchCustomerController() : this(new CustomerQueryServiceImpl(), null, null, null)
        {
        }

        internal SearchCustomerController(
            CustomerQueryService customerQueryService,
            ICurrentSession currentSession,
            ICurrentContext currentContext,
            IExtRequest extRequest) : base(currentSession, currentContext, extRequest)
        {
            this.customerQueryService = customerQueryService;
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index(string groupName, string custName)
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
        public ActionResult UsedCreditPage(int custId, int roleId)
        {
            ViewBag.CustId = custId;

            if (roleId.IsSuper() || roleId.IsMaster())
            {
                var items = customerQueryService.MasterAgentUsedCreditItems(custId, roleId);
                var directMembers = customerQueryService.MemberUsedCreditItems(custId);
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
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SelectClearBalance(int custId)
        {
            ViewBag.CustId = custId;
            var result = customerQueryService.CustomerSelectClearBalance(custId);
            return View(LeftpanelBalanceIndexView, result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
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
        public ActionResult GetDownlineList(bool isDirectDownline, int custId, int pageIndex)
#pragma warning disable S109 // Magic numbers should not be used
            => JsonBase(customerQueryService.GetDownlineList(isDirectDownline, custId, 100, pageIndex));
#pragma warning restore S109 // Magic numbers should not be used

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult RemoveOTP(int custId, string username) =>
            View(LeftPanelOtpIndexView, new { CustId = custId, Username = username });
    }
}