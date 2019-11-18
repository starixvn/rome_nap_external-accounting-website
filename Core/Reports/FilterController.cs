namespace Fanex.ExternalAccount.Core.Reports
{
    using Base;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
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

            foreach (var product in GetProductList())
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
            return JsonBase(GetProductList(), new JsonSerializerSettings());
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