namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using Fanex.ExternalAccount.Excel.Reports.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.Utilities;
    using Requests;
    using Service.Interface.Report;
    using Service.UserControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.SuperWinLossDetailByPT,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class SuperWinLossDetailByPTController : BaseReportController
    {
        private readonly SuperWinLossDetailByPTService services;

        public SuperWinLossDetailByPTController(
            ICurrentSession currentSession,
            SuperWinLossDetailByPTService services,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
            : base(currentSession, reportService, groupConfigurationService)
        {
            this.services = services ?? new SuperWinLossDetailByPTServiceImpl();
        }

        public SuperWinLossDetailByPTController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
        {
            ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());

            return View();
        }

        [AntiXSRFFilter]
        public ActionResult GetData(SuperWinLossDetailByPTRequest request)
        {
            var param = new SuperWinLossDetailByPTParam(
                request.GroupName,
            request.Fromdate,
            request.Todate,
            request.GetProductIds(),
            request.ResourceIds,
            request.PageSize,
            request.PageIndex);

            var viewModel = services.GetSuperWinLossDetailByPT(param);
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportSuperWinLossDetailByPTRequest request)
        {
            ISuperWinLossDetailByPTReportBuilder builder;
            var winlossDetail = services.GetSuperWinLossDetailByPT(new SuperWinLossDetailByPTParam(
                 request.GroupName,
            request.Fromdate,
            request.Todate,
            request.GetProductIds(),
            request.ResourceIds,
            0,
            0));

            builder = new SuperWinLossDetailByPTReportBuilder(
                           winlossDetail.WinLossByUsernames.ToList(),
                           winlossDetail.Total,
                           request.Fromdate,
                           request.Todate);

            var products = GetProductList();
            products = GetPPNameExportExcel(request, products);

            var formatedProducts = request.GetProductNames(products);
            var memoryStream = builder.Build(formatedProducts);

            if (memoryStream != null)
            {
                ExportToExcel(memoryStream, request.FileName);
                memoryStream.Dispose();
            }
        }

        private static List<KeyValuePair<string, string>> GetPPNameExportExcel(ExportSuperWinLossDetailByPTRequest request, List<KeyValuePair<string, string>> products)
        {
            if (string.IsNullOrWhiteSpace(request.ResourceIds))
            {
                if (!Config.IsOfferJackpotProductDetailsGroup(request.GroupName))
                {
                    return products;
                }

                products = products.Where(p => !p.Key.Equals("showPP", StringComparison.OrdinalIgnoreCase)).ToList();
            }

            for (int i = 0, max = products.Count; i < max; i++)
            {
                var product = products[i];

                if (product.Key == "showPP")
                {
                    var userSelectedResourceIds = request.ResourceIds.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);
                    var productDetails = GetProductDetails(request.GroupName)
                        .Where(p => p.ProductKey == product.Key)
                        .ToList();

                    if (productDetails.Count > 0 && !IsEquals(productDetails.Select(p => p.ResourceId), userSelectedResourceIds))
                    {
                        products[i] = new KeyValuePair<string, string>(
                            product.Key,
                            string.Join(", ", productDetails.Where(p => userSelectedResourceIds.Contains(p.ResourceId)).Select(p => p.DisplayName)));
                    }

                    return products;
                }
            }

            return products;
        }

        private static bool IsEquals(IEnumerable<string> source, IEnumerable<string> destination)
            => Enumerable.SequenceEqual(source.OrderBy(id => id), destination.OrderBy(id => id));
    }

    public class ExportSuperWinLossDetailByPTRequest : SuperWinLossDetailByPTRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"SuperWinLossDetailByPT_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}.xls";
    }
}