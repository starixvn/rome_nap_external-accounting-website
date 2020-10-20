namespace Fanex.ExternalAccount.Core.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using Base;
    using Fanex.ExternalAccount.Core.Context;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class FilterController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IExtRequest extRequest;

        public FilterController(IReportService reportService, IExtRequest extRequest)
        {
            this.reportService = reportService ?? new ReportService();
            this.extRequest = extRequest ?? new ExtRequest();
        }

        public FilterController() : this(null, null)
        {
        }

        public ICollection<KeyValuePair<string, bool>> AsCollection(int groupId)
        {
            var list = new List<KeyValuePair<string, bool>>();

            foreach (var product in GetProductList(groupId))
            {
                if (!product.Key.Equals("showAll", StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(new KeyValuePair<string, bool>(product.Key.ToLower(CultureInfo.InvariantCulture),
                        CheckRequest.GetBooleanRequest(product.Key, false)));
                }
            }

            return list;
        }

        public ActionResult ProductsList()
        {
            var groupId = extRequest.GetGroupId();
            var products = GetProductList(groupId);
            var productDetails = GetProductDetails(groupId, extRequest.GetGroupName());

            return JsonBase(new { products, productDetails }, new JsonSerializerSettings());
        }

        public ActionResult GetSuggestedUserName()
        {
            var users = reportService.GetSuggestedUserName(
                CurrentSession.CustId,
                CheckRequest.GetStringRequest("username", string.Empty),
                CheckRequest.GetStringRequest("groupname", string.Empty));

            return JsonBase(users, new JsonSerializerSettings());
        }
    }
}