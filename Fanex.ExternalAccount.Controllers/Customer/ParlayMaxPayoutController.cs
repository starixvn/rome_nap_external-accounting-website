namespace Fanex.ExternalAccount.Controllers.Customer
{
    using Fanex.CustomerSetting.Module.Core.Constants;
    using Fanex.CustomerSetting.Module.Core.Models;
    using Fanex.CustomerSetting.Module.Core.Services.Impl;
    using Fanex.CustomerSetting.Module.Core.Services.Interface;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Core.Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DTO.Requests.ParlayMaxPayout;
    using Fanex.ExternalAccount.Service.Customer.ParlayMaxPayout;
    using System.Collections.Generic;
    using System.Web.Mvc;
    using static Fanex.CustomerSetting.Security.Authenticate.AuthenticateAction;

    public class ParlayMaxPayoutController : BaseController
    {
        private readonly IParlayMaxPayoutService _parlayMaxPayoutService;
        private readonly IServiceBase _serviceBase = new ServiceBase();

        public ParlayMaxPayoutController() : this(null) { }

        public ParlayMaxPayoutController(IParlayMaxPayoutService parlayMaxPayoutService)
        {
            _parlayMaxPayoutService = parlayMaxPayoutService ?? new ParlayMaxPayoutService();
        }

        [ExternalAccountAuthenticate(Function = eFunctions.SportsbookBetting, Policy = AuthenticationType.Read, IsAjaxRequest = false)]
        public ActionResult Index(ParlayMaxPayoutSelectRequest request)
        {
            if (IsMultiple(request))
            {
                request.CustId = GetFirstCustId(request.CustIds);
            }

            var model = _parlayMaxPayoutService.Select(request);
            return PartialViewBase(model);
        }

        [AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.SportsbookBetting, Policy = AuthenticationType.Read)]
        public JsonResult Check(ParlayMaxPayoutUpdateRequest request) => Json(_parlayMaxPayoutService.Check(request));

        [AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.SportsbookBetting, Policy = AuthenticationType.Host)]
        public JsonResult Update(ParlayMaxPayoutUpdateRequest request)
        {
            var checkUmResult = _serviceBase.CheckUM(request.CustId, NonDcsProduct.Sportsbook, AppSettings.DefaultLanguage);
            if (!checkUmResult.IsSuccess)
            {
                return Json(checkUmResult);
            }

            request.UserId = CurrentSession.UserId;
            request.IP = CurrentContext.Ip4Address;
            return Json(_parlayMaxPayoutService.Update(request));
        }

        [AntiXSRFFilter]
        [ExternalAccountAuthenticate(Function = eFunctions.SportsbookBetting, Policy = AuthenticationType.Host)]
        public JsonResult UpdateMultiple(ParlayMaxPayoutUpdateMultipleRequest request)
        {
            var checkUmResult = _serviceBase.CheckUM(GetFirstCustId(request.CustIds), NonDcsProduct.Sportsbook, null);
            if (!checkUmResult.IsSuccess)
            {
                return Json(new List<Result>() { checkUmResult });
            }

            request.UserId = CurrentSession.UserId;
            request.IP = CurrentContext.Ip4Address;
            return Json(_parlayMaxPayoutService.UpdateMultiple(request));
        }

        private static bool IsMultiple(ParlayMaxPayoutSelectRequest request) => request.CustId == 0 && !string.IsNullOrWhiteSpace(request.CustIds);

        private static int GetFirstCustId(string custIds) => int.Parse(custIds.Split(new[] { ',' }, 2)[0]);
    }
}
