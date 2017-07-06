namespace Fanex.ExternalAccount.Core.Reports
{
    using System;
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
            var products = base.GetProductList();
            foreach (KeyValuePair<string, string> product in products)
            {
                if (!product.Key.Equals("showAll", StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(new KeyValuePair<string, bool>(product.Key.ToLower(), CheckRequest.GetBooleanRequest(product.Key, false)));
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
            return this.JsonBase(this.GetProductList(), new Newtonsoft.Json.JsonSerializerSettings());
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