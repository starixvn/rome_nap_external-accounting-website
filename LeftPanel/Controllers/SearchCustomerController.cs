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
        [HttpPost, Authorization(Function = eFunctions.UpdateCredit, Policy = EPolicy.Update)]
        public ActionResult UpdateGivenCreditPost(int id, int uplineId, decimal newGivenCredit)
        {
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
        [Authorization(Function = eFunctions.ClearCredit, Policy = EPolicy.Read)]
        public ActionResult UsedCreditPage(int id, int roleId)
        {
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

        [Authorization(Function = eFunctions.ClearCredit, Policy = EPolicy.Update)]
        public ActionResult ResetMemberCredit(int agentId, bool closedOnly)
        {
            var result = this._service.ResetMemberCredit(CurrentSession.UserId, agentId, closedOnly);
            return this.JsonBase(
                result, 
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [Authorization(Function = eFunctions.UpdateBalance, Policy = EPolicy.Update)]
        public ActionResult UpdateClearBalance(int custId, string custIds, bool isCheckedAll)
        {
            var result = this._service.CustomerInsertClearBalance(custId, custIds, isCheckedAll, CurrentContext.Ip4Address);
            return this.JsonBase(
                result,
                new JsonSerializerSettings
                {
                    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [Authorization(Function = eFunctions.UpdateBalance, Policy = EPolicy.Read)]
        public ActionResult SelectClearBalance(int custId)
        {
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
        [Authorization(Function = eFunctions.CloseCustomer, Policy = EPolicy.Update)]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            if (casinoCustomer.RoleId == Super && !Config.AdminUsersEditSuperStatus.Contains(CurrentSession.UserId))
            {
                return this.JsonBase(
                    new Result(1, ViewLang.DontHavePermission), 
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            this.UpdateCasinoStatus2(custId, close, casinoCustomer.Suspend);
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
        [Authorization(Function = eFunctions.SuspendCustomer, Policy = EPolicy.Update)]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            if (casinoCustomer.RoleId == Super && !Config.AdminUsersEditSuperStatus.Contains(CurrentSession.UserId))
            {
                return this.JsonBase(
                    new Result(1, ViewLang.DontHavePermission), 
                    new JsonSerializerSettings
                    {
                        NullValueHandling = NullValueHandling.Ignore
                    });
            }

            this.UpdateCasinoStatus2(custId, casinoCustomer.Closed, suspend);
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
        [Authorization(Function = eFunctions.AllowOutright, Policy = EPolicy.Update)]
        public ActionResult UpdateOutright(int custId, bool allow)
        {
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
        [Authorization(Function = eFunctions.AllowAutoMPT, Policy = EPolicy.Update)]
        public ActionResult UpdateAutoMPT(int custId, bool allow)
        {
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
        [Authorization(Function = eFunctions.EnableCasino, Policy = EPolicy.Update)]
        public ActionResult UpdateCasino(int custId, int uplineId, bool enable, bool enableDownline, string custName)
        {
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
        [Authorization(Function = eFunctions.EnableBingo, Policy = EPolicy.Update)]
        public ActionResult UpdateBingo(int custId, int uplineId, bool enable, bool enableDownline)
        {
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
        [Authorization(Function = eFunctions.EnableVirtualSports, Policy = EPolicy.Update)]
        public ActionResult UpdateVirtualSports(int custId, bool enable, bool enableDownline)
        {
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
        [Authorization(Function = eFunctions.EnableNumberGame, Policy = EPolicy.Update)]
        public ActionResult UpdateNumberGame(int custId, bool enable, bool enableDownline)
        {
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
        [Authorization(Function = eFunctions.EnableLiveCasino, Policy = EPolicy.Update)]
        public ActionResult UpdateLiveCasino(int custId, bool enable, bool enableDownline)
        {
            return this.JsonBase(
                this._service.EnableLiveCasino(custId, enable, enableDownline),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [Authorization(Function = eFunctions.EnableRacing, Policy = EPolicy.Read)]
        public ActionResult GetEnableRacing(int id, int uplineId, int roleId)
        {
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
        [Authorization(Function = eFunctions.EnableRacing, Policy = EPolicy.Update)]
        public ActionResult UpdateRacing(int id, int uplineId, string txtCustomerObject, int roleId)
        {
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

        [Authorization(Function = eFunctions.UpdateSecurityCode, Policy = EPolicy.Read)]
        public ActionResult GetSecurityCode(int id, int roleId, string username)
        {
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
        [Authorization(Function = eFunctions.UpdateSecurityCode, Policy = EPolicy.Update)]
        public ActionResult UpdateSecurityCode(int id, string code, bool force, int roleId = 1)
        {
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