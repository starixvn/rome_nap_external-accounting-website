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
    using System.Web;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Impl;
    using Fanex.ExternalAccount.Module.LeftPanel.Services.Interface;
    using Fanex.ExternalAccount.Module.LeftPanel.ViewModels;
    using Fanex.ExternalAccount.Core.Base;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// Controller for left panel menu.    
    /// </summary>
    public class SearchCustomerController : BaseController
    {
        private readonly ILeftPanelService service = new LeftPanelService();

        /// <summary>
        /// Gets all info model of left panel.
        /// </summary>
        /// <param name="id">Username to view.</param>
        /// <returns>View contains customer model.</returns>
        public ActionResult Index(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            var model = service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);

            return this.View("~/Areas/CustomerSetting/Views/LeftPanel/Index.cshtml", model);
        }

        [HttpPost]
        public ActionResult ViewDownline(string id = null, string uplineId = "ACCDDD", int pageIndex = 1)
        {
            ViewBag.UserId = CurrentSession.UserId;

            var model = service.GetCustomerInfo(string.IsNullOrWhiteSpace(id) ? string.Empty : id, uplineId, pageIndex);

            return this.JsonBase(model, new JsonSerializerSettings
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
        /// <param name="model">Customer model string.</param>
        /// <returns>JSON object.</returns>
        public ActionResult UpdatePost(int id, int uplineId, string model)
        {
            var customerModel = JsonConvert.DeserializeObject<CustomerModel>(model);

            return this.JsonBase(null);
        }
    }
}