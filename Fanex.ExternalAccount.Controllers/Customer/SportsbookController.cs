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
    using Fanex.ExternalAccount.Service.Interface.Customer;
    using Fanex.ExternalAccount.Service.Impl.Customer;

    /// <summary>
    /// TODO: Update summary.
    /// </summary>
    public class SportsbookController : BaseController
    {
        private ICommissionService commissionService = new CommissionService();

        #region Bet Setting
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSetting(int custId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingMasterGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingAgentGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Master,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditBetSettingMemberGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            Fanex.PTEngine.Services.IBettingLimitService bettingLimitService = new Fanex.PTEngine.Services.BettingLimitService();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Agent,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            IBettingLimit<BettingLimitItem> bettingLimit = bettingLimitService.GetBettingForEdit(currentContext);

            return this.JsonBase(new { betting = bettingLimit, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditBetSettingPost(int custId, int parentId, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            var bettingLimit = JsonConvert.DeserializeObject<BettingLimit>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
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
        #endregion Bet Setting

        #region Edit Commission
        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditCommission(int custId)
        {
            return this.PartialViewBase();
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Read)]
        public ActionResult EditCommissionGet(int custId, int parentId)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;
            settings.ContractResolver = new CamelCasePropertyNamesContractResolver();

            IRequestContext currentContext = new RequestContext()
            {
                FormId = FormType.EditPT,
                UserName = CurrentSession.UserName.ToUpper(),
                CustId = parentId,
                TargetCustId = custId,
                RoleId = (int)CustomerRole.Super,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                SiteName = SiteName,
                Language = "En",
                LangKey = "E",
            };

            ICommissionSetting commissionSetting = commissionService.GetCommission(currentContext);

            return this.JsonBase(new { commission = commissionSetting, context = currentContext }, settings);
        }

        [Authorization(Function = eFunctions.ManageCustomer, Policy = ePolicy.Update)]
        public ActionResult EditCommissionPost(int custId, int parentId, int currentLevel, string txtCustomerObject)
        {
            var settings = new JsonSerializerSettings();
            settings.NullValueHandling = NullValueHandling.Ignore;
            settings.Formatting = Formatting.Indented;

            ICommissionSetting commissionSetting = JsonConvert.DeserializeObject<CommissionSetting>(txtCustomerObject, settings);

            IRequestContext requestContext = new RequestContext()
            {
                CustId = parentId,
                TargetCustId = custId,
                FormId = FormType.EditPT,
                SubAccId = 0,
                AdminId = CurrentSession.UserId,
                IP = CurrentContext.Ip4Address,
                Language = "En"
            };

            DTO.Result result = null;
            if (currentLevel == (int)CustomerRole.Master)
            {
                result = commissionService.UpdateCommissionMaster(commissionSetting, requestContext);
            }
            else if (currentLevel == (int)CustomerRole.Agent)
            {
                result = commissionService.UpdateCommissionAgent(commissionSetting, requestContext);
            }
            else if (currentLevel == (int)CustomerRole.Member)
            {
                result = commissionService.UpdateCommissionMember(commissionSetting, requestContext);
            }
            
            return this.JsonBase(new { ErrorCode = result.ErrorCode, ErrorMessage = result.ErrorMessage });
        }
        #endregion Edit Commission
    }
}