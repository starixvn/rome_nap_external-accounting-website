namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Security.Authenticate;
    using Security.Configurations;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Module.Core.Constants;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;
    using Fanex.Logging.Extensions.Web;
    using Fanex.CustomerSetting.Module.Core.Web.Controllers;

    public class SportsbookController : BaseController
    {
        private readonly Fanex.ExternalAccount.Service.Interface.Customer.ICustomerService externalCustomerService = new Fanex.ExternalAccount.Service.Impl.Customer.CustomerService();

        public ActionResult Index(int custId, int uplineId, int userId, string groupName)
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Index.cshtml", externalCustomerService.IsArchivedCustId(custId));

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public ActionResult EditMultipleBetSetting()
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/MultipleSportsbook/Index.cshtml", false);

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public ActionResult CheckMultipleSupersBetSetting(string sourceUser, string destUsers, string groupName, bool isCopy = true)
        {
            var result = externalCustomerService.CheckMultipleSupersBetSetting(sourceUser, destUsers, groupName, Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId, isCopy);
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, SuperIds = result.ResultData },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}