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
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
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
    }
}