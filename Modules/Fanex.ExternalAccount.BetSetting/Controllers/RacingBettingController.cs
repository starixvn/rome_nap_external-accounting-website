namespace Fanex.CustomerSetting.Module.BetSetting.Controllers
{
    using System;
    using System.Collections;
    using System.Web.Mvc;
    using ExternalAccount.Core.Shared;
    using Base;
    using Security.Authenticate;
    using Security.Configurations;
    using PTRacing.BusinessObjects;
    using PTRacing.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    public class RacingBettingController : BettingControllerBase
    {
        public ActionResult Index(int id, int uplineId, int userId, string siteName)
        {
            var obj = Activator.CreateInstance(Type.GetType(CustomerSettingSection.Instance.AuthenticateFactory)) as IAuthenticate;
            if (!obj.IsBelong(id, null, siteName))
            {
                return ActionDenied(Request);
            }

            return View("~/Areas/CustomerSetting/Views/BetSetting/Racing/Index.cshtml");
        }

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "RacingBetting")]
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

            var bettingLimitService = RacingService;
            var model = bettingLimitService.GetCustomer(new RequestContext
            {
                CustId = uplineId,
                AgentId = 0,
                UserName = string.Empty,
                TargetCustId = id,
                AdminId = userId,
                SubAccId = 0,
                RoleId = Role.Group,
                CurrencyId = 0,
                TargetRoleId = customer.RoleId,
                IsNeutralSite = false,
                FormId = FormType.EditPT,
                IP = HttpContext.Request.UserHostAddress
            });

            return View("~/Areas/CustomerSetting/Views/BetSetting/Racing/Content.cshtml", model);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "RacingBetting")]
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

            var newSetting = JsonConvert.DeserializeObject<Customer>(txtCustomerObject, settings);

            var requestContext = new RequestContext
            {
                CustId = uplineId,
                AgentId = 0,
                UserName = string.Empty,
                TargetCustId = id,
                AdminId = userId,
                SubAccId = 0,
                RoleId = Role.Group,
                CurrencyId = 0,
                TargetRoleId = customer.RoleId,
                IsNeutralSite = false,
                FormId = FormType.EditPT,
                IP = HttpContext.Request.UserHostAddress
            };

            var bettingLimitService = RacingService;
            var oldSetting = bettingLimitService.GetCustomer(requestContext);
            var result = bettingLimitService.SaveCustomer(oldSetting, newSetting, requestContext, new Hashtable());

            return JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage }, settings);
        }
    }
}