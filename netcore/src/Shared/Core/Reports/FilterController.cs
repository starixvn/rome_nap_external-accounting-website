namespace Fanex.ExternalAccount.Core.Reports
{
    using Microsoft.AspNetCore.Mvc;
    using Base;
    using Fanex.ExternalAccount.Core.Context;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Impl.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.Interface.Report.MultipleCurrency;
    using Fanex.ExternalAccount.Service.Products;
    using Newtonsoft.Json;
    using Utilities;

    public class FilterController : BaseController
    {
        private readonly IReportService reportService;
        private readonly IReportByCurrencyService reportByCurrencyService;
        private readonly IProductService productService;
        private readonly IProductDetailService productDetailService;
        private readonly CurrentSession currentSession;

        public FilterController()
            : this(null, null, null, null, null)
        {
        }

        public FilterController(
            IReportService reportService,
            IReportByCurrencyService reportByCurrencyService,
            IProductService productService,
            IProductDetailService productDetailService,
            CurrentSession currentSession)
        {
            this.reportService = reportService ?? new ReportService();
            this.reportByCurrencyService = reportByCurrencyService ?? new ReportByCurrencyService();
            this.productService = productService ?? new ProductService();
            this.productDetailService = productDetailService ?? new ProductDetailService();
            this.currentSession = currentSession ?? new CurrentSession();
        }

        public IActionResult ProductsList(bool isWinLossDetail)
        {
            var products = productService.GetProducts(ExtRequest.GetGroupName(), ExtRequest.GetGroupId(), isWinLossDetail);
            var productDetails = productDetailService.GetProductDetails(products, currentSession.ParentGroupName);

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