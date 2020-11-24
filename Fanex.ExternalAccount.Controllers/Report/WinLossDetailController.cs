namespace Fanex.ExternalAccount.Controllers.Report
{
    using Core.Context;
    using Core.Shared;
    using CustomerSetting.Security.Authenticate;
    using DTO.Report;
    using Excel.Reports.WinLossDetailReport;
    using Fanex.ExternalAccount.Utilities;
    using Requests;
    using Service.Impl.Report;
    using Service.Interface.Report;
    using Service.UserControl;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Web.Mvc;

    [ExternalAccountAuthenticate(Function = eFunctions.WinlossDetail,
        Policy = AuthenticateAction.AuthenticationType.Read, IsAjaxRequest = false)]
    public class WinLossDetailController : BaseReportController
    {
        private readonly WinLossDetailService winLossDetailService;

        public WinLossDetailController(
            CurrentSession currentSession,
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
            request.Viewlevel++;
            var winlossDetail = GetDataFromDB(request);
            request.Viewlevel--;
            switch (request.Viewlevel)
            {
                case Role.Member:
                    {
                        builder = new MemberWinLossDetailReportBuilder(
                            (List<WinLossDetailAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Agent:
                    {
                        builder = new AgentWinLossDetailReportBuilder(
                            (List<WinLossDetailMAg>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                case Role.Master:
                    {
                        builder = new WinLossDetailReportBuilder(
                            (List<WinLossDetail>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
                default:
                    {
                        builder = new WinLossDetailExtraReportBuilder(
                            (List<WinLossDetailExtra>)winlossDetail.GetType().GetProperty("rows").GetValue(winlossDetail, null),
                            request.Viewlevel,
                            request.Fromdate,
                            request.Todate);

                        break;
                    }
            }

            var products = GetProductList(request.GroupId);
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
            var extraPTs = winLossDetailService.GetExtraPT(request, currentSession.ParentGroupName);

            return Json(extraPTs);
        }

        private dynamic GetDataFromDB(WinLossDetailRequest request)
        {
            var toolTipsProducts = new
            {
                liveCasino = BuildTooltipsForLiveCasino(request.GroupName)
            };

            return request.IsSearch()
                ? winLossDetailService.SearchWinLossDetail(
                    new SearchWinLossDetailRequest(request, toolTipsProducts))
                : winLossDetailService.DrillDownWinLossDetail(
                    new DrillDownWinLossDetailRequest(request, toolTipsProducts));
        }

        private static List<KeyValuePair<string, string>> GetPPNameExportExcel(ExportWinLossDetailRequest request, List<KeyValuePair<string, string>> products)
        {
            if (string.IsNullOrWhiteSpace(request.ResourceIds))
            {
                if (!Config.IsOfferJackpotProductDetailsGroup(request.MainGroupName))
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
                    var productDetails = GetProductDetails(request.GroupId, request.MainGroupName)
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
            $"{Viewlevel.DisplayRoleName()}WinLossDetail_{Fromdate.ToString(DateFormat)}_{Todate.ToString(DateFormat)}.xls";
    }
}