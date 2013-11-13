// --------------------------------------------------------------------------
// <copyright file="SearchCustomerController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2013.07.25">Created</change>
// </history>

namespace Fanex.ExternalAccount.Module.LeftPanel.Controllers
{
    using System;
    using System.Collections;
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

    /// <summary>
    /// Controller for left panel menu.    
    /// </summary>
    public class SearchCustomerController : BaseController
    {
        private readonly ILeftPanelService _service = new LeftPanelService();

        /// <summary>
        /// Gets all info model of left panel.
        /// </summary>
        /// <param name="id">Username to view.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>View contains customer model.</returns>
        public ActionResult Index(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            return this.ViewBase("~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml");
        }

        /// <summary>
        /// Gets all info model of left panel.
        /// </summary>
        /// <param name="id">Username to view.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>View contains customer model.</returns>
        public ActionResult GetData(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            if (!this.IsBelong(uplineId, 0, id, true))
            {
                return this.Content("var model = null;");
            }

            var model = this._service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);
            if (model != null)
            {
                model.GetProductStatuses(uplineId, id);
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

        /// <summary>
        /// Views all down-lines of customer.
        /// </summary>
        /// <param name="id">Id of customer.</param>
        /// <param name="uplineId">Upper id of customer.</param>
        /// <param name="pageIndex">Current page index.</param>
        /// <returns>JSON object.</returns>
        [HttpPost]
        public ActionResult ViewDownline(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            if (!this.IsBelong(uplineId, 0, id, true))
            {
                return this.Content("var model = null;");
            }

            var model = this._service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);

            return this.JsonBase(
                model,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates info of panel.
        /// </summary>
        /// <param name="id">Customer id to update.</param>
        /// <param name="uplineId">Up-line id of customer.</param>
        /// <param name="newGivenCredit">New given credit.</param>
        /// <returns>JSON object.</returns>
        [HttpPost, ExternalAccountAuthenticate(Function = eFunctions.UpdateCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateGivenCreditPost(int id, int uplineId, decimal newGivenCredit)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var model = this._service.UpdateGivenCredit(id, uplineId, newGivenCredit);

            return this.JsonBase(
                model, 
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Gets used credit page.
        /// </summary>
        /// <param name="id">Upper customer id.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult UsedCreditPage(int id, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
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

        /// <summary>
        /// Resets member credit.
        /// </summary>
        /// <param name="agentId">Agent customer id.</param>
        /// <param name="closedOnly">Resets closed account only.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.ClearCredit, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
        {
            if (!this.IsBelong(Request["groupName"], agentId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var result = this._service.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly);
            return this.JsonBase(
                result, 
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Clears balance.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="custIds">Customer id list.</param>
        /// <param name="isCheckedAll">Checks all or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var result = this._service.CustomerInsertClearBalance(custId, custIds, isCheckedAll, CurrentContext.Ip4Address);
            return this.JsonBase(
                result,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [ExternalAccountAuthenticate(Function = eFunctions.UpdateBalance, Policy = AuthenticateAction.AuthenticationType.Read)]
        public ActionResult SelectClearBalance(int custId)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.CustId = custId;
            var result = this._service.CustomerSelectClearBalance(custId);
            return this.PartialViewBase(result, "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml");
        }

        /// <summary>
        /// Updates closed status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="close">Closed or not.</param>
        /// <param name="clearCredit">Clear credit or not.</param>
        /// <param name="custName">Customer username.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.CloseCustomer, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            if (casinoCustomer != null && casinoCustomer.RoleId == Super && !Config.AdminUsersEditSuperStatus.Contains(CurrentSession.UserId))
            {
                return this.JsonBase(
                    new Result(1, ViewLang.DontHavePermission), 
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            if (casinoCustomer != null)
            {
                this.UpdateCasinoStatus2(custId, close, casinoCustomer.Suspend);
            }

            return this.JsonBase(
                this._service.CloseCutomer(custId, close, clearCredit, custName), 
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates suspended status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="suspend">Suspended or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.SuspendCustomer, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            if (casinoCustomer != null && casinoCustomer.RoleId == Super && !Config.AdminUsersEditSuperStatus.Contains(CurrentSession.UserId))
            {
                return this.JsonBase(
                    new Result(1, ViewLang.DontHavePermission), 
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            if (casinoCustomer != null)
            {
                this.UpdateCasinoStatus2(custId, casinoCustomer.Closed, suspend);
            }

            return this.JsonBase(
                this._service.SuspendCustomer(custId, suspend),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates outright status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="allow">Allow or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.AllowOutright, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateOutright(int custId, bool allow)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            return this.JsonBase(
                this._service.AllowOutright(custId, allow),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates auto master PT status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="allow">Allow or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.AllowAutoMPT, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateAutoMPT(int custId, bool allow)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            return this.JsonBase(
                this._service.AutoMasterPT(custId, allow),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates casino status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="enable">Enable or not.</param>
        /// <param name="enableDownline">Enable down-lines or not.</param>
        /// <param name="custName">Customer username.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableCasino, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return this.JsonBase(
                    this._service.EnableCasino(custId, enable, enableDownline, casinoCustomer.CasinoSync, custName),
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                return this.JsonBase(new { IsSuccess = ex.Message.Contains("Not casino") ? true : false, ErrorCode = 1, ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Updates bingo status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="enable">Enable or not.</param>
        /// <param name="enableDownline">Enable down-lines or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableBingo, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateBingo(int custId, int uplineId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            try
            {
                var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
                return this.JsonBase(
                    this._service.EnableBingo(custId, enable, casinoCustomer.BingoSync, enableDownline),
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }
            catch (Exception ex)
            {
                return this.JsonBase(new { IsSuccess = ex.Message.Contains("Not casino") ? true : false, ErrorCode = 1, ErrorMessage = ex.Message });
            }
        }

        /// <summary>
        /// Updates enable virtual sports status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="enable">Allow or not.</param>
        /// <param name="enableDownline">Enable down-lines or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableVirtualSports, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateVirtualSports(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            return this.JsonBase(
                this._service.EnableVirtualSports(custId, enable, enableDownline),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates enable number game status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="enable">Allow or not.</param>
        /// <param name="enableDownline">Enable down-lines or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableNumberGame, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateNumberGame(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            return this.JsonBase(
                this._service.EnableNumberGame(custId, enable, enableDownline),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Updates enable live casino status.
        /// </summary>
        /// <param name="custId">Customer id.</param>
        /// <param name="enable">Allow or not.</param>
        /// <param name="enableDownline">Enable down-lines or not.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableLiveCasino, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateLiveCasino(int custId, bool enable, bool enableDownline)
        {
            if (!this.IsBelong(Request["groupName"], custId, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            return this.JsonBase(
                this._service.EnableLiveCasino(custId, enable, enableDownline),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        /// <summary>
        /// Gets enable racing page.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetEnableRacing(int id, int uplineId, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;

            return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/LeftPanel/EnableRacing/Index.cshtml");
        }

        /// <summary>
        /// Gets racing status.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="uplineId">Upper customer id.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>ActionResult object.</returns>        
        public ActionResult GetRacingData(int id, int uplineId, int roleId)
        {
            ViewBag.roleId = roleId;
            ViewBag.id = id;
            ViewBag.uplineId = uplineId;
            var bettingLimitService = new CustomerService();
            var model = bettingLimitService.GetCustomer(new Fanex.PTRacing.BusinessObjects.RequestContext
            {
                CustId = uplineId, // upline id
                AgentId = 0, // unknow
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

        /// <summary>
        /// Edits the bet setting post.
        /// </summary>
        /// <param name="id">The identifier.</param>
        /// <param name="uplineId">The up-line identifier.</param>
        /// <param name="txtCustomerObject">The text customer object.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>ActionResult object.</returns>
        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.EnableRacing, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateRacing(int id, int uplineId, string txtCustomerObject, int roleId)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            var newSetting = JsonConvert.DeserializeObject<Fanex.PTRacing.BusinessObjects.Customer>(txtCustomerObject, settings);

            var requestContext = new Fanex.PTRacing.BusinessObjects.RequestContext
            {
                CustId = uplineId, // upline id
                AgentId = 0, // unknow
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

        /// <summary>
        /// Gets security code page.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <param name="username">Customer username.</param>
        /// <returns>ActionResult object.</returns>
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode, Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult GetSecurityCode(int id, int roleId, string username)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.custId = id;
            ViewBag.roleId = roleId;
            ViewBag.Username = username;
            return this.PartialViewBase(null, "~/Areas/CustomerSetting/Views/LeftPanel/SecurityCode/Index.cshtml");
        }

        /// <summary>
        /// Updates security code.
        /// </summary>
        /// <param name="id">Customer id.</param>
        /// <param name="code">Security code.</param>
        /// <param name="force">Force to change or not.</param>
        /// <param name="roleId">Customer role id.</param>
        /// <returns>ActionResult object.</returns>
        [HttpPost]
        [ExternalAccountAuthenticate(Function = eFunctions.UpdateSecurityCode, Policy = AuthenticateAction.AuthenticationType.Host)]
        public ActionResult UpdateSecurityCode(int id, string code, bool force, int roleId = 1)
        {
            if (!this.IsBelong(Request["groupName"], id, null, true))
            {
                return this.JsonBase(new { ErrorCode = "1", ErrorMessage = ViewLang.DontHavePermission, IsSuccess = false });
            }

            string hashCode = Encryption.CfsEnCode(code);
            if (roleId != Member)
            {
                var md5 = Fanex.Security.Cryptography.ComputeHash("MD5", id.ToString(), "1qaw3+{:P)");
                hashCode = Fanex.Security.Cryptography.ComputeHash("SHA1", code, md5);
            }

            Result result = this._service.ResetSecurityCode(id, hashCode, force);
            return this.JsonBase(
                result, 
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}