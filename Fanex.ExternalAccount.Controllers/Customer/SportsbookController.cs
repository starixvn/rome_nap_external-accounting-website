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

        [AuthorizationAttribute(Function = eFunctions.ManageGroup, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingMasterGet(int custId)
        {
            JsonSerializerSettings settings = new JsonSerializerSettings();
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
                RoleId = 4,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = "",
                Language = "En",
                LangKey = "E",
            };

            bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            //var ret = JsonConvert.SerializeObject(bettingLimit, settings);

            return this.JsonBase(bettingLimit);
        }
    }
}