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
namespace Fanex.ExternalAccount.Core.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Utilities;
    using WinLossDetail;

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
                if (!this.IsHiddenProduct(product) && !product.Id.Equals("showAll"))
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
                if (!this.IsHiddenProduct(product))
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

        public bool IsHiddenProduct(ProductElementConfiguration product)
        {
            if (product.Hide)
            {
                return true;
            }

            if (product.ShowForGroups.Equals("*", StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }

            var groups = product.ShowForGroups.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
            var requestURL = Request != null ? Request.UrlReferrer.AbsolutePath.Split(new string[] { "/" }, StringSplitOptions.RemoveEmptyEntries) : new string[0];
            var groupName = requestURL.Length == 0 || !CurrentSession.IsInternalAdmin ? CurrentSession.CurrencyGroup : requestURL[requestURL.Length - 1];

            foreach (var group in groups)
            {
                if (groupName.Equals(group, StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Finance", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("Internal", StringComparison.OrdinalIgnoreCase) ||
                    groupName.Equals("IT", StringComparison.OrdinalIgnoreCase))
                {
                    return false;
                }
            }
                        
            return true;
        }        
    }
}