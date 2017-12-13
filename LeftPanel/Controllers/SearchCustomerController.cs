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
    using IBCUtils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using PTRacing.BusinessObjects;
    using PTRacing.Services;
    using Security;
    using Service.Impl.Customer.Casino;
    using Services.Impl;
    using Services.Interface;
    using Utilities;
    using ViewModels;
    using Result = DTO.Result;

    public class SearchCustomerController : BaseController
    {
        private readonly ILeftPanelService _service = new LeftPanelService();

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index()
        {
            ViewBag.UserId = CurrentSession.UserId;

            return View("~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml");
        }

        public ActionResult GetData(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;
            var checkCustomerBelongToSpecialSiteResult = CheckCustomerBelongToSpecialSite(id);
            if (!IsAdminHasPermissionOnCustomer(checkCustomerBelongToSpecialSiteResult.IsSpecialSite
                                                || checkCustomerBelongToSpecialSiteResult.IsSpecialHeadline))
            {
                return Content(
                    "var model = null; $(\"div[data-ng-controller='NoInfoController']\").html(\"You don\'t have permission to access\")");
            }

            if (!IsBelong(uplineId, 0, id, true))
            {
                return Content("var model = null;");
            }

            var model = _service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId,
                pageIndex);

            model?.GetProductStatuses(CurrentSession.UserId, uplineId, id);

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
        [AntiXSRFFilter]
        public ActionResult ViewDownline(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            if (!IsBelong(uplineId, 0, id, true))
            {
                return Content("var model = null;");
            }

            var model = _service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId,
                pageIndex);

            return JsonBase(model);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateCredit,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGivenCreditPost(int id, int uplineId, decimal newGivenCredit)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            var model = _service.UpdateGivenCredit(id, uplineId, newGivenCredit);

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult UsedCreditPage(int id, int roleId)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.CustId = id;

            if (roleId == Super || roleId == Master)
            {
                var items = _service.MasterAgentUsedCreditItems(id, roleId);
                var model = new MasterAgentUsedCreditModel
                {
                    MasterAgentUsedCreditItems = items
                };

                return PartialViewBase(model,
                    "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MasterAgentCredit.cshtml");
            }
            else
            {
                var items = _service.MemberUsedCreditItems(id);
                var model = new MemberUsedCreditModel
                {
                    MemberUsedCreditItems = items
                };

                return PartialViewBase(model, "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MemberCredit.cshtml");
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
        {
            if (!IsBelong(Request["groupName"], agentId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _service.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly);
            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _service.CustomerInsertClearBalance(custId, custIds, isCheckedAll, CurrentContext.Ip4Address);
            return JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SelectClearBalance(int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.CustId = custId;
            var result = _service.CustomerSelectClearBalance(custId);
            return PartialViewBase(result, "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml");
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer?.RoleId == Super && !close
                && !IsAuthorized(CurrentSession.UserId, eFunctions.OpenSuper,
                    AuthenticateAction.AuthenticationType.Host, Request))
            {
                return JsonBase(new Result(1, ViewLang.DontHavePermission));
            }

            if (casinoCustomer?.CasinoSync == true)
            {
                UpdateCasinoClosed(custId, close);
            }

            return JsonBase(_service.CloseCutomer(custId, close, clearCredit, custName));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer?.CasinoSync == true)
            {
                UpdateCasinoSuspended(custId, suspend);
            }

            return JsonBase(_service.SuspendCustomer(custId, suspend));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.AllowOutright,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateOutright(int custId, bool allow) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : JsonBase(_service.AllowOutright(custId, allow));

        [ExternalAccountAuthenticate(Function = eFunctions.AllowAutoMPT,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateAutoMPT(int custId, bool allow, int uplineId) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : JsonBase(_service.AutoMasterPt(custId, allow, uplineId));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableCasino,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return JsonBase(_service.EnableCasino(custId, enable, enableDownline, casinoCustomer.CasinoSync,
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
        public ActionResult UpdateESports(int custId, bool enable, bool enableDownline) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : JsonBase(_service.EnableESports(custId, enable, enableDownline));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableNumberGame,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateNumberGame(int custId, bool enable, bool enableDownline) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : JsonBase(_service.EnableNumberGame(custId, enable, enableDownline));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableLiveCasino,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateLiveCasino(int custId, bool enable, bool enableDownline) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : JsonBase(_service.EnableLiveCasino(custId, enable, enableDownline));

        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetEnableRacing(int id, int uplineId, int roleId)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;

            return PartialViewBase("~/Areas/CustomerSetting/Views/LeftPanel/EnableRacing/Index.cshtml");
        }

        public ActionResult GetRacingData(int id, int uplineId, int roleId)
        {
            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;
            var bettingLimitService = new CustomerService();
            var model = bettingLimitService.GetCustomer(new RequestContext
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
        public ActionResult UpdateRacing(int id, int uplineId, string txtCustomerObject, int roleId)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

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
                TargetCustId = id,
                AdminId = CurrentSession.UserId,
                SubAccId = 0,
                RoleId = roleId + 1,
                CurrencyId = 0,
                TargetRoleId = roleId,
                IsNeutralSite = false,
                FormId = FormType.EditPT,
                IP = CurrentContext.Ip4Address
            };

            var bettingLimitService = new CustomerService();
            var oldSetting = bettingLimitService.GetCustomer(requestContext);
            var result = bettingLimitService.SaveCustomer(oldSetting, newSetting, requestContext, new Hashtable());

            return JsonBase(new {result.ErrorCode, result.ErrorMessage}, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetSecurityCode(int id, int roleId, string username)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            ViewBag.custId = id;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;
            return PartialViewBase(viewName: "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml");
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSecurityCode(int id, string code, bool force, int roleId = 2)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            var hashCode = Encryption.CfsEnCode(code);
            if (roleId != Member)
            {
                var md5 = Cryptography.ComputeHash("MD5", id.ToString(), "1qaw3+{:P)"); //DevSkim: ignore DS126858
                hashCode = Cryptography.ComputeHash("SHA1", code, md5); //DevSkim: ignore DS126858
            }

            var result = _service.ResetSecurityCode(id, hashCode, force || roleId != (int) CustomerRole.Super);
            return JsonBase(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableHRFixedOdds,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateHRFixedOdds(int id, bool enable, bool enableDownline)
        {
            if (!IsBelong(Request["groupName"], id, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _service.EnableHrFixedOdds(id, enable, enableDownline);

            return JsonBase(result);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetOnlineList(int custId)
        {
            var model = new OnlineList();
            foreach (DataRow row in _service.GetOnlineList(custId))
            {
                model.Onlines.Add(new OnlineInfo
                {
                    IP = new ObjectInfo {Text = CustomerModelExtensions.CheckVNIP(row.ToString("ip"))},
                    LoginTime = new ObjectInfo
                    {
                        Text = Utils.ToDateTime(row["reload_time"]).ToStringExt("MM/dd/yyyy HH:mm:ss tt")
                    }
                });
            }

            return JsonBase(model);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetDownlineList(int custId, int pageIndex)
        {
            var downlines = _service.GetDownlineList(custId, 100, pageIndex);
            var model = new DownlineList();
            var totalPages = LeftPanelService.TotalPages;

            if (downlines?.Rows.Count <= 0)
            {
                return JsonBase(model);
            }

            if (downlines?.Rows != null)
            {
                foreach (DataRow item in downlines?.Rows)
                {
                    model.Downlines.Add(new ObjectInfo {Text = item["username"].ToString()});
                }
            }

            model.CurrentPage = pageIndex;

            if (totalPages > 1)
            {
                for (var i = 1; i <= totalPages; i++)
                {
                    model.Pages.Add(new ObjectInfo {Value = i, Text = i.ToString()});
                }
            }
            else
            {
                model.Pages.Add(new ObjectInfo {Value = 1, Text = "1"});
            }

            return JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableGoldDeluxe,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGoldDeluxe(int custId, int uplineId, bool enable, bool enableDownline,
            string custName)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            try
            {
                return JsonBase(_service.UpdateGoldDeluxe(custId, enable, enableDownline, custName,
                    CurrentSession.CustId));
            }
            catch (Exception ex)
            {
                return JsonBase(new {IsSuccess = false, ErrorCode = 1, ErrorMessage = ex.Message});
            }
        }

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult RemoveOTP(int custId, string username) =>
            !IsBelong(Request["groupName"], custId, null, true)
                ? ActionDenied(Request)
                : PartialViewBase(new {CustId = custId, Username = username},
                    "~/Areas/CustomerSetting/Views/LeftPanel/OTP/Index.cshtml");

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP,
            Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult RemoveOTP(int custId)
        {
            if (!IsBelong(Request["groupName"], custId, null, true))
            {
                return ActionDenied(Request);
            }

            var result = _service.RemoveOtp(custId);
            return JsonBase(result);
        }
    }
}