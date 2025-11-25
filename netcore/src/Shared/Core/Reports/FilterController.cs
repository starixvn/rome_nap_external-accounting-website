namespace Fanex.ExternalAccount.Core.Reports
{
    using Microsoft.AspNetCore.Mvc;
    using Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Service.Products;
    using Newtonsoft.Json;
    using Utilities;
    using Microsoft.Extensions.Configuration;
    using Fanex.ExternalAccount.Core.Authorization;

    public class FilterController(
        IReportService reportService,
        IReportByCurrencyService reportByCurrencyService,
        IProductService productService,
        IProductDetailService productDetailService,
        ICurrentSession currentSession,
        ICurrentContext currentContext,
        IExtRequest extRequest,
        IConfiguration configuration,
        IAuthorizationService authorizationService) : BaseController(currentSession, currentContext, extRequest, configuration, authorizationService)
    {
        private readonly IReportService reportService = reportService;
        private readonly IReportByCurrencyService reportByCurrencyService = reportByCurrencyService;
        private readonly IProductService productService = productService;
        private readonly IProductDetailService productDetailService = productDetailService;

        public IActionResult ProductsList(bool isWinLossDetail)
        {
            var products = productService.GetProducts(ExtRequest.GetGroupName(), ExtRequest.GetGroupId(), isWinLossDetail);
            var productDetails = productDetailService.GetProductDetails(products, CurrentSession.ParentGroupName);

            return JsonBase(new { products, productDetails }, new JsonSerializerSettings());
        }

        public IActionResult GetSuggestedUserName()
        {
            var users = reportService.GetSuggestedUserName(
                CurrentSession.CustId,
                CheckRequest.GetStringRequest("username", string.Empty),
                CheckRequest.GetStringRequest("groupname", string.Empty));

            return JsonBase(users, new JsonSerializerSettings());
        }

        public IActionResult CurrencyList()
        {
            var currencies = reportByCurrencyService.GetCurrencyList(ExtRequest.GetGroupName());

            return JsonBase(currencies, new JsonSerializerSettings());
        }
    }
}