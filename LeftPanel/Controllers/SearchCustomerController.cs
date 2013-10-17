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
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.ExternalAccount.Module.LeftPanel.Extensions;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Service.Impl.Customer.Casino;
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

            var model = this._service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);
            if (model != null)
            {
                model.GetProductStatuses(uplineId, id);
            }

            return this.ViewBase("~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml", model);
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
        [HttpPost, Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
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
        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
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

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
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

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
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

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Read)]
        public ActionResult SelectClearBalance(int custId)
        {
            ViewBag.CustId = custId;
            var result = this._service.CustomerSelectClearBalance(custId);
            return this.PartialViewBase(result, "~/Areas/CustomerSetting/Views/LeftPanel/Balance/Index.cshtml");
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult UpdateCloseCutomer(int custId, int uplineId, bool close, bool clearCredit, string custName)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            this.UpdateCasinoStatus2(custId, close, casinoCustomer.Suspend);
            return this.JsonBase(
                this._service.CloseCutomer(custId, close, clearCredit, custName), 
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }

        [Authorization(Function = eFunctions.EditCustomer, Policy = EPolicy.Update)]
        public ActionResult UpdateSuspendCutomer(int custId, int uplineId, bool suspend)
        {
            var casinoCustomer = CasinoManager.Instance.GetCustInfoIBC(custId, uplineId);
            this.UpdateCasinoStatus2(custId, casinoCustomer.Closed, suspend);
            return this.JsonBase(
                this._service.SuspendCustomer(custId, suspend),
                new JsonSerializerSettings
                {
                    NullValueHandling = NullValueHandling.Ignore
                });
        }
    }
}