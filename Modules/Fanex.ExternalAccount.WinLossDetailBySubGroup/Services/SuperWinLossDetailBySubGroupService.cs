using Fanex.ExternalAccount.DataAccess.Products;

namespace Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Services
{
    using System.Linq;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Criteria;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.DataAccesses;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Models;
    using Fanex.ExternalAccount.SuperWinLossDetailBySubGroup.Requests;
    using Fanex.ExternalAccount.Utilities;

    public interface ISuperWinLossDetailBySubGroupService
    {
        SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroupReport(
            GetSuperWinLossDetailBySubGroupRequest request);

        string ConcatProductName(string productIds, string resourceIds, string groupName);
    }

    public class SuperWinLossDetailBySubGroupService : ISuperWinLossDetailBySubGroupService
    {
        private readonly ISuperWinLossDetailBySubGroupDataAccess dataAccess;
        private readonly IProductService productService;
        private readonly IProductDetailService productDetailService;
        private readonly IReportService reportService;

        public SuperWinLossDetailBySubGroupService(
            ISuperWinLossDetailBySubGroupDataAccess dataAccess = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null,
            IReportService reportService = null)
        {
            this.dataAccess = dataAccess ?? new SuperWinLossDetailBySubGroupDataAccess();
            this.productService = productService ?? new ProductService();
            this.productDetailService = productDetailService ?? new ProductDetailService();
            this.reportService = reportService ?? new ReportService(
                productService: this.productService,
                productDetailService: this.productDetailService);
        }

        public SuperWinLossDetailBySubGroupReport GetSuperWinLossDetailBySubGroupReport(
            GetSuperWinLossDetailBySubGroupRequest request)
        {
            var validProductIds = productService.ValidateProductIds(request.GroupName, request.GroupId, Utils.ToEnumerableInt(request.ProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, request.ProductDetails, request.GroupName);
            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, request.ProductDetails, request.GroupName);
          
            var criteria = new GetSuperWinLossDetailBySubGroupCriteria(
                Utils.JoinToString(validProductIds),
                productDetailsXML,
                request.GroupName,
                request.SubGroup,
                dataAccess.GetSubGroupList(request.GroupId, request.SubGroup),
                request.FromDate,
                request.ToDate,
                ReportService.BalanceUpDate,
                ReportService.MoveBetsDate,
                request.PageIndex + 1,
                request.PageSize);

            var report = dataAccess.GetSuperWinLossDetailBySubGroup(criteria);

            if (report.TotalItems > 0)
            {
                report.SuperWinLossDetails = report.SuperWinLossDetails.GroupBy(
                    w => w.SubGroup,
                    (key, superWinLossDetails) =>
                    {
                        var total = new SuperWinLossDetailBySubGroupModel();

                        var superWinLossDetailsList = superWinLossDetails.ToList();
                        superWinLossDetailsList.ForEach(item => total += item);

                        total.SubGroup = key ?? "Unassigned Supers";
                        total.SuperWinLossDetails = superWinLossDetailsList;

                        return total;
                    });
            }

            return report;
        }

        public string ConcatProductName(string productIds, string resourceIds, string groupName)
            => reportService.ConcatProductName(productIds, resourceIds, groupName);
    }
}
