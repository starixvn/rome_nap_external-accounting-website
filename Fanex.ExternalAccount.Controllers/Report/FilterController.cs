// --------------------------------------------------------------------------
// <copyright file="FilterController.cs" company="Nexcel Solutions Vietnam">
// Copyright (c) Nexcel Solutions Vietnam. All rights reserved.
// </copyright>
// <summary>
// For current session info
// </summary>
// --------------------------------------------------------------------------
// <history>
//   <change who="Andy.Nguyen" date="2012.12.14">Created</change>
// </history>
namespace Fanex.ExternalAccount.Controllers.Report
{
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;

    /// <summary>
    /// Report controller.
    /// </summary>
    public partial class FilterController : BaseController
    {
        /// <summary>
        /// Gets list of request from filter condition.
        /// </summary>
        /// <returns>ICollection object.</returns>
        public ICollection<KeyValuePair<string, bool>> AsCollection()
        {
            var list = new List<KeyValuePair<string, bool>>();
            var products = ProductsConfiguration.Instance.ProductCollection;
            foreach (ProductElementConfiguration product in products)
            {
                if (!product.Hide && !product.Id.Equals("showAll"))
                {
                    list.Add(new KeyValuePair<string, bool>(product.Id.ToLower(), CheckRequest.GetBooleanRequest(product.Id, true)));
                }
            }

            return list;
        }

        /// <summary>
        /// Gets product list.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult ProductsList()
        {
            var list = new List<KeyValuePair<string, string>>();
            var products = ProductsConfiguration.Instance.ProductCollection;
            foreach (ProductElementConfiguration product in products)
            {
                if (!product.Hide)
                {
                    list.Add(new KeyValuePair<string, string>(product.Id, product.Name));
                }
            }

            return this.JsonBase(list, new Newtonsoft.Json.JsonSerializerSettings());
        }

        /// <summary>
        /// Gets suggested usernames.
        /// </summary>
        /// <returns>ActionResult object.</returns>
        public ActionResult GetSuggestedUserName()
        {
            string groupName = CheckRequest.GetStringRequest("groupname", string.Empty);

            if (string.IsNullOrEmpty(groupName) || !this.IsBelong(groupName, 0, null, true))
            {
                return this.JsonBase(new { username = string.Empty });
            }

            IReportService reportService = new ReportService();
            var users = reportService.GetSuggestedUserName(
                CurrentSession.CustId, 
                CheckRequest.GetStringRequest("username", string.Empty), 
                CheckRequest.GetStringRequest("groupname", string.Empty));
            
            return this.JsonBase(users, new Newtonsoft.Json.JsonSerializerSettings());
        }
    }
}