namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Base;
    using Security.Authenticate;
    using Security.Configurations;
    using PTEngine.BusinessObjects;
    using PTEngine.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class SportsbookController : BettingControllerBase
    {
        public ActionResult Index(int custId, int uplineId, int userId, string groupName)
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Index.cshtml");

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public ActionResult Content(int custId, int uplineId, int userId, string groupName)
        {
            var customer = CustomerInfo(custId, userId);

            if (customer.RoleId.IsNotSuper())
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino; 
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            var bettingLimitService = SportsBookService;

            var currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                CustId = uplineId,
                TargetCustId = custId,
                SubAccId = 0,
                AdminId = userId,
                SiteName = groupName,
                Language = "En",
                LangKey = "E",
            };

            var bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Content.cshtml", bettingLimit);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "SportsbookBetting")]
        public ActionResult EditBetSettingPost(int custId, int uplineId, int userId, string txtCustomerObject)
        {
            var customer = CustomerInfo(custId, userId);

            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            if (customer.RoleId.IsNotSuper())
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);

            var requestContext = new RequestContext()
            {
                CustId = userId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = userId,
                IP = Request.UserHostAddress,
                Language = "En"
            };

            var bettingLimitService = SportsBookService;

            var result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage }, settings);
        }
    }
}