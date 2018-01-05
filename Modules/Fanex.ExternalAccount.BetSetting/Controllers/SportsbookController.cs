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
        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public ActionResult Content(int id, int uplineId, int userId, string siteName)
        {
            var customer = CustomerInfo(id, userId);

            if (customer.RoleId.IsNotSuper())
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.SyncCasino = customer.SyncCasino;
            ViewBag.SyncBingo = customer.SyncBingo;
            ViewBag.SyncLiveCasino = customer.SyncLiveCasino;
            ViewBag.HideGoldDeluxe = customer.HideGoldDeluxe;

            var bettingLimitService = SportsBookService;

            var currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                CustId = uplineId,
                TargetCustId = id,
                SubAccId = 0,
                AdminId = userId,
                SiteName = siteName,
                Language = "En",
                LangKey = "E",
            };

            var bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Content.cshtml", bettingLimit);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "SportsbookBetting")]
        public ActionResult EditBetSettingPost(int id, int uplineId, int userId, string txtCustomerObject, string siteName = "ACCUT")
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            var customer = CustomerInfo(id, userId);

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
                TargetCustId = id,
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