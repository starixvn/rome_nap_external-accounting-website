namespace Fanex.ExternalAccount.Controllers.Customer.Sportsbook
{
    using Core.Base;
    using Core.Context;
    using Core.Shared;
    using Service.Impl.Customer;
    using System.Collections.Generic;
    using Microsoft.AspNetCore.Mvc;
    using AuthenticationType = CustomerSetting.Security.Authenticate.AuthenticateAction.AuthenticationType;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.Sportsbook, Policy = AuthenticationType.Read)]
    public class SportsbookSettingController : BaseController
    {
        private const int NG2Start = 213;
        private const int NG2End = 220;
   
        private readonly ISiteDBConfigService siteDBConfigService;

        public SportsbookSettingController(ISiteDBConfigService siteDBConfigService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, AuthorizationService authorizationService)
         : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.siteDBConfigService = siteDBConfigService;
        }

        public IActionResult ViewPtInfo(int custId)
        {
            var exclusiveRefSportIds = new List<int>(8);

            for (var i = NG2Start; i <= NG2End; i++)
            {
                exclusiveRefSportIds.Add(i);
            }

            ViewData["ExclusiveRefSportIds"] = exclusiveRefSportIds.ToArray();
            ViewData["SiteDB"] = siteDBConfigService.GetSiteDBConfigByCustId(custId);

            return View("~/Views/BettypeReference/Index.cshtml");
        }
    }
}