namespace Fanex.ExternalAccount.Controllers.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Module.Core.Web;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Utilities;
    using Fanex.ExternalAccount.Utilities.Reports.WinLossDetail;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;

        public WinLossDetailController(
            ICurrentSession currentSession,
            WinLossDetailService winLossDetailService,
            IReportService reportService,
            IGroupConfigurationService groupConfigurationService)
            : base(currentSession, reportService, groupConfigurationService)
        {
            this.winLossDetailService = winLossDetailService ?? new WinLossDetailServiceImpl();
        }

        public WinLossDetailController() : this(null, null, null, null)
        {
        }

        public ActionResult Index()
        {
            ViewBag.CanViewExtraPT = CanView(Config.GroupsCanViewExtraPT());

            return View();
        }

        [AntiXSRFFilter]
        public ActionResult GetData(WinLossDetailRequest request)
        {
            var viewModel = GetDataFromDB(request);
            return Json(viewModel);
        }

        public void ExportWinLossDetailToExcel(ExportWinLossDetailRequest request)
        {
            IWinLossDetailReportBuilder builder;
            request.viewlevel++;
            var winlossDetail = GetDataFromDB(request);
            request.viewlevel--;
            switch (request.viewlevel)
            {
                case Role.Member:
                    {
                        builder = new MemberWinLossDetailReportBuilder(
                            (List<WinLossDetailAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailReportBuilder(
                            (List<WinLossDetailMAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                case Role.Master:
                    {
                        builder = new WinLossDetailReportBuilder(
                            (List<WinLossDetail>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailExtraReportBuilder(
                            (List<WinLossDetailExtra>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.viewlevel,
                            request.fromdate,
                            request.todate);

                        break;
                    }
            }

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

        [AntiXSRFFilter]
        public ActionResult GetExtraPT(ViewExtraPTRequest request)
        {
            var extraPTs = winLossDetailService.GetExtraPT(request);

            return Json(extraPTs);
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = BuildTooltipsForLiveCasino()
            };

            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, toolTipsProducts))
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }

        private string BuildTooltipsForLiveCasino()
        {
            var liveCasino = new List<GroupConfigurationModel>
            {
                new GroupConfigurationModel{ProductId=ProductConstants.AllBet,ProductName="AllBet"},
                new GroupConfigurationModel{ProductId=ProductConstants.SexyGaming,ProductName="AE Casino"},
                new GroupConfigurationModel{ProductId=ProductConstants.SAGaming,ProductName="SA Gaming"},
                new GroupConfigurationModel{ProductId=ProductConstants.GTCasino,ProductName="IBCBet Live Casino"},
                new GroupConfigurationModel{ProductId=ProductConstants.BBin,ProductName="BBIN"},
                new GroupConfigurationModel{ProductId=ProductConstants.ION,ProductName="ION"},
                new GroupConfigurationModel{ProductId=ProductConstants.WM,ProductName="WM"}
            };

            var offeredLiveCasino = liveCasino.Where(
                i => groupConfigurationService.IsShowByProductAndGroup(i.ProductId, currentSession.CurrencyGroup));

            if (offeredLiveCasino != null && offeredLiveCasino.Any())
            {
                var liveCasinoName = string.Join(", ", offeredLiveCasino.Select(i => i.ProductName));

                return string.Format(ViewLang.Tooltip_NetTurnover_LiveCasino, liveCasinoName);
            }

            return string.Empty;
        }

        private static List<KeyValuePair<string, string>> GetPPNameExportExcel(ExportWinLossDetailRequest request, List<KeyValuePair<string, string>> products)
        {
            if (string.IsNullOrWhiteSpace(request.ResourceIds))
            {
                if (!Config.IsOfferJackpotProductDetailsGroup(request.groupName))
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
                    var productDetails = GetProductDetails(request.groupName)
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

    public class ExportWinLossDetailRequest : WinLossDetailRequest
    {
        private const string DateFormat = "yyyyMMdd";

        public string FileName =>
            $"{viewlevel.DisplayRoleName()}WinLossDetail_{fromdate.ToString(DateFormat)}_{todate.ToString(DateFormat)}.xls";
    }
}