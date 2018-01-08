namespace Fanex.ExternalAccount.Core.Reports
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Web.Mvc;
    using Authorization;
    using Base;
    using Newtonsoft.Json;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Utilities;

    public class FilterController : BaseController
    {
        private readonly AuthorizationService authorizationService;
        private readonly IReportService reportService;

        public FilterController(IReportService reportService, AuthorizationService authorizationService)
        {
            this.reportService = reportService ?? new ReportService();
            this.authorizationService = authorizationService ?? new AuthorizationServiceImpl();
        }

        public FilterController() : this(null, null)
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

        public ActionResult ProductsList() => JsonBase(GetProductList(), new JsonSerializerSettings());

        public ActionResult GetSuggestedUserName()
        {
            var groupName = CheckRequest.GetStringRequest("groupname", string.Empty);

            if (string.IsNullOrEmpty(groupName) || !authorizationService.IsBelong(groupName, 0, null, true))
            {
                return JsonBase(new { username = string.Empty });
            }

            var users = reportService.GetSuggestedUserName(
                CurrentSession.CustId,
                CheckRequest.GetStringRequest("username", string.Empty),
                CheckRequest.GetStringRequest("groupname", string.Empty));

            return JsonBase(users, new JsonSerializerSettings());
        }
    }
}