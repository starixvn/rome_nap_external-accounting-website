namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Base;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Report;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.OandaExchangeRate,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class OandaExchangeController : BaseController
    {
        private readonly IExchangeService exchangeService;

        public OandaExchangeController() : this(null) { }

        public OandaExchangeController(IExchangeService exchangeService)
        {
            this.exchangeService = exchangeService ?? new ExchangeService();
        }

        public ActionResult Index()
        {
            return View();
        }

        [AntiXSRFFilter]
        public ActionResult GetExchangeRate(GetExchangeRateRequest request)
        {
            var exchangeRates = exchangeService.GetOandaExchangeRate(request.FromDate, request.ToDate, request.GroupName);

            return Json(exchangeRates);
        }
    }


}
