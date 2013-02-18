// -----------------------------------------------------------------------
// <copyright file="SportsbookController.cs" company="">
// TODO: Update copyright text.
// </copyright>
// -----------------------------------------------------------------------

namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using System;
    using System.Web.Mvc;
    using Fanex.Caching;
    using Fanex.ExternalAccount.Controllers.Base;
    using Fanex.ExternalAccount.Controllers.Context;
    using Fanex.ExternalAccount.DTO;
    using Fanex.PTEngine.BusinessObjects;
    using Fanex.PTEngine.Services;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Serialization;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportsbookController : BaseController
    {
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingMaster(int custId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingMasterGet(int custId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            IRequestContext currentContext = new RequestContext();
            IBettingLimit<BettingLimitItem> bettingLimit = new BettingLimit();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = "",
                Language = "En",
                LangKey = "E",
            };

            bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditBetSettingMasterPost(int custId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            IBettingLimitService bettingLimitService = new BettingLimitService();

            Fanex.PTEngine.BusinessObjects.Result result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingAgent(int custId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingAgentGet(int custId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            IRequestContext currentContext = new RequestContext();
            IBettingLimit<BettingLimitItem> bettingLimit = new BettingLimit();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Master,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = "",
                Language = "En",
                LangKey = "E",
            };

            bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditBetSettingAgentPost(int custId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);
            IRequestContext requestContext = new RequestContext()
            {
                CustId = CurrentSession.UserId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            IBettingLimitService bettingLimitService = new BettingLimitService();

            Fanex.PTEngine.BusinessObjects.Result result = bettingLimitService.UpdateBetting(bettingLimit, requestContext);

            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }
    }
}