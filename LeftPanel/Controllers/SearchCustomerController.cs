namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.Mvc;
    using Core.Base;
    using Core.Context;
    using CustomerSetting.Security.Authenticate;
    using Extensions;
    using Core.Shared;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTRacing.BusinessObjects;
    using PTRacing.Services;
    using Service.Impl.Customer.Casino;
    using Services;
    using Utilities;
    using ViewModels;
    using Result = DTO.Result;

    public class SearchCustomerController : BaseController
    {
        private const string MasterAgentCreditViewl = "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MasterAgentCredit.cshtml";
        private const string MemberCreditView = "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MemberCredit.cshtml";
        private const string SecurityCodeIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml";
        private const string LeftPanelIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml";
        private const string LeftpanelBalanceIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml";
        private const string LeftPanelOtpIndexView = "~/Areas/CustomerSetting/Views/LeftPanel/OTP/Index.cshtml";

        private readonly CustomerQueryService _service = new CustomerQueryServiceImpl();
        private readonly ICustomerService customerService = new CustomerService();
        private readonly CustomerCommandService customerCommandService = new CustomerCommandServiceImpl();

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
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

            var customerInfo = _service.SearchCustomer(
                userId,
                custName,
                uplineId);

            return View(LeftPanelIndexView, customerInfo);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGivenCreditPost(int custId, int uplineId, decimal newGivenCredit)
        {
            var model = customerCommandService.UpdateGivenCredit(custId, uplineId, newGivenCredit);

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult UsedCreditPage(int custId, int roleId)
        {
            ViewBag.CustId = custId;

            if (roleId.IsSuper() || roleId.IsMaster())
            {
                var items = _service.MasterAgentUsedCreditItems(custId, roleId);
                var model = new MasterAgentUsedCreditModel
                {
                    MasterAgentUsedCreditItems = items
                };

                return PartialViewBase(model, MasterAgentCreditViewl);
            }
            else
            {
                var items = _service.MemberUsedCreditItems(custId);
                var model = new MemberUsedCreditModel
                {
                    MemberUsedCreditItems = items
                };

                return PartialViewBase(model, MemberCreditView);
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
        {
            var result = customerCommandService.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly);
            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
        {
            var result = customerCommandService.CustomerInsertClearBalance(custId, custIds, isCheckedAll, CurrentContext.Ip4Address);
            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SelectClearBalance(int custId)
        {
            ViewBag.CustId = custId;
            var result = customerCommandService.CustomerSelectClearBalance(custId);
            return PartialViewBase(result, LeftpanelBalanceIndexView);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer?.RoleId == Role.Super && !close
                && !AuthorizationService.IsAuthorized(CurrentSession.UserId, eFunctions.OpenSuper, AuthenticateAction.AuthenticationType.Host))
            {
                return JsonBase(new Result(1, ViewLang.DontHavePermission));
            }

            if (casinoCustomer?.CasinoSync == true)
            {
                UpdateCasinoClosed(custId, close);
            }

            return JsonBase(customerCommandService.CloseCutomer(custId, close, clearCredit, custName));
        }

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
        public ActionResult UpdateOutright(int custId, bool allow) =>
            JsonBase(customerCommandService.AllowOutright(custId, allow));

        [ExternalAccountAuthenticate(Function = eFunctions.AllowAutoMPT,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateAutoMPT(int custId, bool allow, int uplineId) =>
            JsonBase(customerCommandService.AutoMasterPt(custId, allow, uplineId));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableCasino,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return JsonBase(customerCommandService.EnableCasino(custId, enable, enableDownline, casinoCustomer.CasinoSync,
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

        [ExternalAccountAuthenticate(Function = eFunctions.EnableESports,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateESports(int custId, bool enable, bool enableDownline)
            => JsonBase(customerCommandService.EnableESports(custId, enable, enableDownline));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableNumberGame,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateNumberGame(int custId, bool enable, bool enableDownline) =>
            JsonBase(customerCommandService.EnableNumberGame(custId, enable, enableDownline));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableLiveCasino,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateLiveCasino(int custId, bool enable, bool enableDownline) =>
            JsonBase(customerCommandService.EnableLiveCasino(custId, enable, enableDownline));

        public ActionResult GetRacingData(int id, int uplineId, int roleId)
        {
            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;

            var model = customerService.GetCustomer(new RequestContext
            {
                CustId = uplineId,
                AgentId = 0,
                UserName = string.Empty,
                TargetCustId = id,
                AdminId = CurrentSession.UserId,
                SubAccId = 0,
                RoleId = roleId + 1,
                CurrencyId = 0,
                TargetRoleId = roleId,
                IsNeutralSite = false,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address
            });

            var result = "var model = " + JsonConvert.SerializeObject(
                             model,
                             Formatting.None,
                             new JsonSerializerSettings
                             {
                                 ContractResolver = new CamelCasePropertyNamesContractResolver(),
                                 NullValueHandling = NullValueHandling.Ignore
                             }) + ";";

            return Content(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateRacing(int custId, int uplineId, string txtCustomerObject, int roleId)
        {
            var settings = new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore,
                ContractResolver = new CamelCasePropertyNamesContractResolver()
            };

            var newSetting = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = uplineId,
                AgentId = 0,
                UserName = string.Empty,
                TargetCustId = custId,
                AdminId = CurrentSession.UserId,
                SubAccId = 0,
                RoleId = roleId + 1,
                CurrencyId = 0,
                TargetRoleId = roleId,
                IsNeutralSite = false,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address
            };

            var oldSetting = customerService.GetCustomer(requestContext);
            var result = customerService.SaveCustomer(oldSetting, newSetting, requestContext, new Hashtable());

            return JsonBase(new { result.ErrorCode, result.ErrorMessage }, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetSecurityCode(int custId, int roleId, string username)
        {
            ViewBag.custId = custId;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;

            return PartialView(SecurityCodeIndexView);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSecurityCode(int custId, string code, bool force, int roleId = 2)
        {
            var result = customerCommandService.UpdateSecurityCode(custId, code, force, roleId);

            return JsonBase(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableHRFixedOdds,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateHRFixedOdds(int id, bool enable, bool enableDownline)
        {
            var result = customerCommandService.EnableHrFixedOdds(id, enable, enableDownline);

            return JsonBase(result);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetOnlineList(int custId)
        {
            var model = _service.GetOnlineList(custId);

            return JsonBase(model);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetDownlineList(int custId, int pageIndex)
        {
            int totalPages;
            var downlines = _service.GetDownlineList(custId, 100, pageIndex, out totalPages);

            var model = new DownlineList();

            if (downlines?.Rows.Count <= 0)
            {
                return JsonBase(model);
            }

            if (downlines?.Rows != null)
            {
                foreach (DataRow item in downlines?.Rows)
                {
                    model.Downlines.Add(new ObjectInfo { Text = item["username"].ToString() });
                }
            }

            model.CurrentPage = pageIndex;

            if (totalPages > 1)
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    model.Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                }
            }
            else
            {
                model.Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
            }

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableGoldDeluxe,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGoldDeluxe(int custId, bool enable, bool enableDownline,
            string custName)
        {
            try
            {
                return JsonBase(customerCommandService.UpdateGoldDeluxe(custId, enable, enableDownline, custName,
                    CurrentSession.CustId));
            }
            catch (Exception ex)
            {
                return JsonBase(new { IsSuccess = false, ErrorCode = 1, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult RemoveOTP(int custId, string username) =>
            PartialViewBase(new { CustId = custId, Username = username },
                    LeftPanelOtpIndexView);

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult RemoveOTP(int custId)
        {
            var result = customerCommandService.RemoveOtp(custId);
            return JsonBase(result);
        }
    }
}