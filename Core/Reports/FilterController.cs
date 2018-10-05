namespace Fanex.ExternalAccount.Core.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using System.Web.Mvc;
    using Base;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class FilterController : BaseController
    {
        private readonly IReportService reportService;

        public FilterController(IReportService reportService)
        {
            this.reportService = reportService ?? new ReportService();
        }

        public FilterController() : this(null)
        {
        }

        public ICollection<KeyValuePair<string, bool>> AsCollection()
        {
            var list = new List<KeyValuePair<string, bool>>();

            foreach (var product in GetProductList(Enumerable.Empty<int>()))
            {
                if (!product.Key.Equals("showAll", StringComparison.CurrentCultureIgnoreCase))
                {
                    list.Add(new KeyValuePair<string, bool>(product.Key.ToLower(CultureInfo.InvariantCulture),
                        CheckRequest.GetBooleanRequest(product.Key, false)));
                }
            }

            return list;
        }

        public ActionResult ProductsList(bool isHideInactiveProduct)
        {
            var excludeProductIds = reportService.GetExcludeProductIds(ExtRequest.GetGroupName());

            return JsonBase(GetProductList(excludeProductIds, isHideInactiveProduct), new JsonSerializerSettings());
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