namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Fanex.ExternalAccount.Core.Base;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;
    using Security.Authenticate;
    using Fanex.ExternalAccount.Core.Context;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;
    using Fanex.ExternalAccount.Service.Interface.Customer;

    public class SportsbookController : BaseController
    {
        private readonly ICustomerService externalCustomerService;

        protected SportsbookController(ICustomerService externalCustomerService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.externalCustomerService = externalCustomerService;
        }

        public IActionResult Index(int custId, int uplineId, int userId, string groupName)
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Index.cshtml", externalCustomerService.IsArchivedCustId(custId));

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public IActionResult EditMultipleBetSetting()
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/MultipleSportsbook/Index.cshtml", false);

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public IActionResult CheckMultipleSupersBetSetting(string sourceUser, string destUsers, string groupName, bool isCopy = true)
        {
            var result = externalCustomerService.CheckMultipleSupersBetSetting(sourceUser, destUsers, groupName, Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId, isCopy);
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, SuperIds = result.ResultData },
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}