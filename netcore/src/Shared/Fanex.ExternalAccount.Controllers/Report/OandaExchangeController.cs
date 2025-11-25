namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Base;
    using Fanex.CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Controllers.Requests;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Report;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    [ExternalAccountAuthenticate(Function = eFunctions.OandaExchangeRate,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class OandaExchangeController : BaseController
    {
        private readonly IExchangeService exchangeService;

        public OandaExchangeController(IExchangeService exchangeService, ICurrentSession currentSession, ICurrentContext currentContext, IExtRequest extRequest, IConfiguration configuration, IAuthorizationService authorizationService) : base(currentSession, currentContext, extRequest, configuration, authorizationService)
        {
            this.exchangeService = exchangeService;
        }

        public IActionResult Index()
        {
            return View();
        }

        [ValidateAntiForgeryToken]
        public IActionResult GetExchangeRate(GetExchangeRateRequest request)
        {
            var exchangeRates = exchangeService.GetOandaExchangeRate(request.FromDate, request.ToDate, request.GroupName);

            return Ok(exchangeRates);
        }
    }


}
