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
    using Fanex.CustomerSetting.Module.Core.Web;
    using Fanex.CustomerSetting.Module.Core.Constants;
    using System.Linq;
    using System.Collections.Generic;
    using System.Collections;

    public class SportsbookController : BettingControllerBase
    {
        private readonly Fanex.ExternalAccount.Service.Interface.Customer.ICustomerService externalCustomerService = new Fanex.ExternalAccount.Service.Impl.Customer.CustomerService();

        public ActionResult Index(int custId, int uplineId, int userId, string groupName)
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/Sportsbook/Index.cshtml", externalCustomerService.IsArchivedCustId(custId));

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBettingNew")]
        public ActionResult EditMultipleBetSetting()
            => PartialView("~/Areas/CustomerSetting/Views/BetSetting/MultipleSportsbook/Index.cshtml", false);

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBetting")]
        public ActionResult Content(int custId, int uplineId, int userId, string groupName)
        {
            var customer = CustomerInfo(custId, userId);

            if (customer.RoleId.IsNotSuper())
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            ViewBag.Username = customer.Username;
            ViewBag.HideCasino = customer.HideCasino;
            ViewBag.DefaultMinBetValue = GetDefaultMinBetValue();

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

        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBettingNew")]
        public ActionResult EditMultipleBetSettingContent(int? custId, string groupName = null)
        {
            var customer = CustomerInfo(custId.Value, Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId);

            if (customer.RoleId > 0 && customer.RoleId.IsNotSuper())
            {
                return PartialView("~/Areas/CustomerSetting/Views/Security/AccessDenied.cshtml");
            }

            var bettingLimitService = SportsBookService;

            var currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                TargetCustId = custId.Value,
                SubAccId = 0,
                SiteName = groupName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = null;
            if (custId == 0)
            {
                bettingLimit = new BettingLimit { BettingLimitItems = new List<BettingLimitItem>() };
                bettingLimit.Populate();

                foreach (BettingLimitItem bettingLimitItem in bettingLimit.BettingLimitItems)
                {
                    bettingLimitItem.MinBet = 0;
                    bettingLimitItem.MaxBet = 0;
                    bettingLimitItem.MaxPerMatch = 0;
                    bettingLimitItem.MaxPayoutPerMatch = 0;
                }
            }
            else
            {
                bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);
            }

            return PartialView("~/Areas/CustomerSetting/Views/BetSetting/MultipleSportsbook/Content.cshtml", bettingLimit);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "SportsbookBetting")]
        public ActionResult EditBetSettingPost(int custId, int uplineId, int userId, string txtCustomerObject)
        {
            var checkUmResult = serviceBase.CheckUM(custId, NonDcsProduct.Sportsbook, AppSession.CurrentSession.LangKey);
            if (!checkUmResult.IsSuccess)
            {
                return UmJsonResult(checkUmResult);
            }

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

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Host, "SportsbookBettingNew")]
        public ActionResult EditMultipleBetSettingPost(int custId, string txtCustomerObject, string para)
        {
            var checkUmResult = serviceBase.CheckUM(custId, NonDcsProduct.Sportsbook, AppSession.CurrentSession.LangKey);
            if (!checkUmResult.IsSuccess)
            {
                return UmJsonResult(checkUmResult);
            }

            var param = new Hashtable();
            if (!string.IsNullOrWhiteSpace(para))
            {
                for (var i = 0; i < para.Length; i++)
                {
                    param.Add(i, para[i].ToString());
                }
            }

            var customer = CustomerInfo(custId, Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId);
            var settings = new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() };

            if (customer.RoleId.IsNotSuper())
            {
                return JsonBase(new { ErrorCode = 1, ErrorMessage = "You're not authorized on this function" }, settings);
            }

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);

            var requestContext = new RequestContext()
            {
                CustId = Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                IP = Request.UserHostAddress,
                Language = "En"
            };

            var bettingLimitService = SportsBookService;
            var result = bettingLimitService.UpdateMultipleBettingForSuper(bettingLimit, requestContext, param);

            return JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage }, settings);
        }

        [HttpPost]
        [AuthenticateAction(AuthenticateAction.AuthenticationType.Read, "SportsbookBettingNew")]
        public ActionResult CheckMultipleSupersBetSetting(string sourceUser, string destUsers, string groupName, bool isCopy = true)
        {
            var result = externalCustomerService.CheckMultipleSupersBetSetting(sourceUser, destUsers, groupName, Fanex.ExternalAccount.Core.Context.CurrentSession.Instance.CustId, isCopy);
            return JsonBase(new { result.ErrorCode, result.ErrorMessage, SuperIds = result.ResultData }, 
                new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ContractResolver = new CamelCasePropertyNamesContractResolver() });
        }
    }
}