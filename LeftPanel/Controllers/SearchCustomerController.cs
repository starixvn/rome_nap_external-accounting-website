namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System;
    using System.Collections;
    using System.Data;
    using System.Web.Mvc;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.PTRacing.Services;
    using IBCUtils;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SearchCustomerController : BaseController
    {
        private readonly ILeftPanelService _service = new LeftPanelService();

        [ExternalAccountAuthenticate(Function = eFunctions.CustomerList, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            return View("~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml");
        }

        public ActionResult GetData(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;
            var checkCustomerBelongToSpecialSiteResult = base.CheckCustomerBelongToSpecialSite(id);
            if (!base.IsAdminHasPermissionOnCustomer(id, checkCustomerBelongToSpecialSiteResult.IsSpecialSite || checkCustomerBelongToSpecialSiteResult.IsSpecialHeadline))
            {
                return this.Content("var model = null; $(\"div[data-ng-controller='NoInfoController']\").html(\"You don\'t have permission to access\")");
            }

            if (!this.IsBelong(uplineId, 0, id, true))
            {
                return this.Content("var model = null;");
            }

            var model = this._service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);
            if (model != null)
            {
                model.GetProductStatuses(CurrentSession.UserId, uplineId, id);
            }

            string result = "var model = " + JsonConvert.SerializeObject(
                model,
                Formatting.None,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                }) + ";";

            return this.Content(result);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult ViewDownline(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            if (!this.IsBelong(uplineId, 0, id, true))
            {
                return this.Content("var model = null;");
            }

            var model = this._service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);

            return this.JsonBase(model);
        }

        [HttpPost, ExternalAccountAuthenticate(Function = eFunctions.UpdateCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGivenCreditPost(int id, int uplineId, decimal newGivenCredit)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var model = this._service.UpdateGivenCredit(id, uplineId, newGivenCredit);

            return this.JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult UsedCreditPage(int id, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            ViewBag.CustId = id;

            if (roleId == Super || roleId == Master)
            {
                var items = this._service.MasterAgentUsedCreditItems(id, roleId);
                var model = new MasterAgentUsedCreditModel
                {
                    MasterAgentUsedCreditItems = items
                };

                return this.PartialViewBase(model, "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MasterAgentCredit.cshtml");
            }
            else
            {
                var items = this._service.MemberUsedCreditItems(id);
                var model = new MemberUsedCreditModel
                {
                    MemberUsedCreditItems = items
                };

                return this.PartialViewBase(model, "~/Areas/CustomerSetting/Views/LeftPanel/UsedCredit/MemberCredit.cshtml");
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
        {
            if (!this.IsBelong(Request["groupName"], agentId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var result = this._service.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly);
            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var result = this._service.CustomerInsertClearBalance(custId, custIds, isCheckedAll, CurrentContext.Ip4Address);
            return this.JsonBase(result);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult SelectClearBalance(int custId)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            ViewBag.CustId = custId;
            var result = this._service.CustomerSelectClearBalance(custId);
            return this.PartialViewBase(result, "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml");
        }

        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            if (casinoCustomer != null && casinoCustomer.RoleId == Super && !close
                && !IsAuthorized(CurrentSession.UserId, eFunctions.OpenSuper, AuthenticateAction.AuthenticationType.Host, Request))
            {
                return this.JsonBase(new Result(1, ViewLang.DontHavePermission));
            }

            if (casinoCustomer != null && casinoCustomer.CasinoSync)
            {
                this.UpdateCasinoClosed(custId, close);
            }

            return this.JsonBase(this._service.CloseCutomer(custId, close, clearCredit, custName));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);

            if (casinoCustomer != null && casinoCustomer.CasinoSync)
            {
                this.UpdateCasinoSuspended(custId, suspend);
            }

            return this.JsonBase(this._service.SuspendCustomer(custId, suspend));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.AllowOutright, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateOutright(int custId, bool allow)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.JsonBase(this._service.AllowOutright(custId, allow));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.AllowAutoMPT, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateAutoMPT(int custId, bool allow, int uplineId)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.JsonBase(this._service.AutoMasterPT(custId, allow, uplineId));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableCasino, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return this.JsonBase(this._service.EnableCasino(custId, enable, enableDownline, casinoCustomer.CasinoSync, custName));
            }
            catch (Exception ex)
            {
                return this.JsonBase(new { IsSuccess = ex.Message.Contains("Not casino") ? true : false, ErrorCode = 1, ErrorMessage = ex.Message });
            }
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableESports, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateESports(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.JsonBase(this._service.EnableESports(custId, enable, enableDownline));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableNumberGame, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateNumberGame(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.JsonBase(this._service.EnableNumberGame(custId, enable, enableDownline));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableLiveCasino, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateLiveCasino(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.JsonBase(this._service.EnableLiveCasino(custId, enable, enableDownline));
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetEnableRacing(int id, int uplineId, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;

            return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/LeftPanel/EnableRacing/Index.cshtml");
        }

        public ActionResult GetRacingData(int id, int uplineId, int roleId)
        {
            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;
            var bettingLimitService = new CustomerService();
            var model = bettingLimitService.GetCustomer(new Fanex.PTRacing.BusinessObjects.RequestContext
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
                FormId = Fanex.PTRacing.BusinessObjects.FormType.EditPT,
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

            return this.Content(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateRacing(int id, int uplineId, string txtCustomerObject, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var newSetting = JsonConvert.DeserializeObject<Fanex.PTRacing.BusinessObjects.Customer>(txtCustomerObject, settings);

            var requestContext = new Fanex.PTRacing.BusinessObjects.RequestContext
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
                FormId = Fanex.PTRacing.BusinessObjects.FormType.EditPT,
                IP = CurrentContext.Ip4Address
            };

            var bettingLimitService = new Fanex.PTRacing.Services.CustomerService();
            var oldSetting = bettingLimitService.GetCustomer(requestContext);
            var result = bettingLimitService.SaveCustomer(oldSetting, newSetting, requestContext, new Hashtable());

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage }, settings);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetSecurityCode(int id, int roleId, string username)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            ViewBag.custId = id;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;
            return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml");
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateSecurityCode(int id, string code, bool force, int roleId = 2)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            string hashCode = Encryption.CfsEnCode(code);
            if (roleId != Member)
            {
                var md5 = Fanex.Security.Cryptography.ComputeHash("MD5", id.ToString(), "1qaw3+{:P)");
                hashCode = Fanex.Security.Cryptography.ComputeHash("SHA1", code, md5);
            }

            Result result = this._service.ResetSecurityCode(id, hashCode, force || roleId != (int)PTRacing.BusinessObjects.CustomerRole.Super);
            return this.JsonBase(result);
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableHRFixedOdds, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateHRFixedOdds(int id, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            Result result = this._service.EnableHRFixedOdds(id, enable, enableDownline);

            return this.JsonBase(result);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetOnlineList(int custId)
        {
            var model = new OnlineList();
            var onlines = this._service.GetOnlineList(custId);

            foreach (DataRow row in onlines)
            {
                model.Onlines.Add(new OnlineInfo
                {
                    IP = new ObjectInfo { Text = CustomerModelExtensions.CheckVNIP(row.ToString("ip")) },
                    LoginTime = new ObjectInfo { Text = Utils.ToDateTime(row["reload_time"]).ToStringExt("MM/dd/yyyy HH:mm:ss tt") }
                });
            }

            return this.JsonBase(model);
        }

        [HttpPost]
        [AntiXSRFFilter]
        public ActionResult GetDownlineList(int custId, int pageIndex)
        {
            var downlines = this._service.GetDownlineList(custId, 100, pageIndex);
            var model = new DownlineList();
            var totalPages = LeftPanelService.TotalPages;

            if (downlines != null && downlines.Rows.Count > 0)
            {
                foreach (DataRow item in downlines.Rows)
                {
                    model.Downlines.Add(new ObjectInfo { Text = item["username"].ToString() });
                }

                model.CurrentPage = pageIndex;

                if (totalPages > 1)
                {
                    for (int i = 1; i <= totalPages; i++)
                    {
                        model.Pages.Add(new ObjectInfo { Value = i, Text = i.ToString() });
                    }
                }
                else
                {
                    model.Pages.Add(new ObjectInfo { Value = 1, Text = "1" });
                }
            }

            return this.JsonBase(model);
        }

        [ExternalAccountAuthenticate(Function = eFunctions.EnableGoldDeluxe, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult UpdateGoldDeluxe(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            try
            {
                return this.JsonBase(this._service.UpdateGoldDeluxe(custId, enable, enableDownline, custName, CurrentSession.CustId));
            }
            catch (Exception ex)
            {
                return this.JsonBase(new { IsSuccess = false, ErrorCode = 1, ErrorMessage = ex.Message });
            }
        }

        [HttpGet]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult RemoveOTP(int custId, string username)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            return this.PartialViewBase(new { CustId = custId, Username = username }, "~/Areas/CustomerSetting/Views/LeftPanel/OTP/Index.cshtml");
        }

        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.RemoveOTP, Policy = AuthenticateAction.AuthenticationType.Host)]
        [AntiXSRFFilter]
        public ActionResult RemoveOTP(int custId)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.ActionDenied(this.Request);
            }

            var result = this._service.RemoveOTP(custId);
            return this.JsonBase(result);
        }
    }
}