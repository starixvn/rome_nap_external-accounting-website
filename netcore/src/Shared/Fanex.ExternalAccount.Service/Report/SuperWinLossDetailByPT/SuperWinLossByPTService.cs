namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Products;
    using Fanex.ExternalAccount.Utilities;

    public interface ISuperWinLossDetailByPTService
    {
        SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param);
        SuperWinLossDetailMCByPTModel GetSuperWinLossMCDetailByPT(SuperWinLossDetailByPTParam param);
    }

    public class SuperWinLossDetailByPTServiceImpl : ISuperWinLossDetailByPTService
    {
        private readonly SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess;
        private readonly SuperWinLossMCDetailByPTDao superWinLossMCDetailByPTDataAccess;
        private readonly IProductService productService;
        private readonly IProductDetailService productDetailService;
        private readonly IReportUtils reportUtils;

        public SuperWinLossDetailByPTServiceImpl(
            SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess = null,
            SuperWinLossMCDetailByPTDao superWinLossMCDetailByPTDataAccess = null,
            IProductService productService = null,
            IProductDetailService productDetailService = null,
            IReportUtils reportUtils = null)
        {
            this.superWinLossDetailByPTDataAccess = superWinLossDetailByPTDataAccess ?? new SuperWinLossDetailByPTDataAccessImpl();
            this.superWinLossMCDetailByPTDataAccess = superWinLossMCDetailByPTDataAccess ?? new SuperWinLossMCDetailByPTDaoImpl();
            this.productService = productService ?? new ProductService();
            this.productDetailService = productDetailService ?? new ProductDetailService();
            this.reportUtils = reportUtils ?? new ReportUtils();
        }

        public SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }
            
            if (!param.IsValid())
            {
                return new SuperWinLossDetailByPTModel();
            }

            var validProductIds = productService.ValidateProductIds(param.GroupName, param.GroupId, Utils.ToEnumerableInt(param.UserSelectedProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, param.ResourceIds, param.GroupName);
            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, param.ResourceIds, param.GroupName);

            var criteria = new SuperWinLossDetailByPTCriteria(
                param.GroupName,
                param.FromDate,
                param.ToDate,
                reportUtils.BalanceUpDate,
                reportUtils.MoveBetsDate,
                Utils.JoinToString(validProductIds),
                productDetailsXML,
                param.PageSize,
                param.PageIndex);

            var model = superWinLossDetailByPTDataAccess.GetWinLossByBetType(criteria);

            if (model.WinLossByProducts.Any())
            {
                var superIds = model.WinLossByProducts.Select(item => item.CustId).Distinct();

                var postionTakings = superWinLossDetailByPTDataAccess
                    .GetSuperPositionTakingList(new SuperPTCriteria(Utils.JoinToString(superIds)));

                var winLossByUsernames = BuildWinLossByUsernames(model.WinLossByProducts, postionTakings)
                    .OrderByDescending(winloss => winloss.Total.Turnover)
                    .ToList();

                if (model.DormantAccount != null)
                {
                    winLossByUsernames.Add(new SuperWinLossDetailByUsernameModel(model.DormantAccount));
                }

                return new SuperWinLossDetailByPTModel(
                    winLossByUsernames,
                    model.GrandTotal,
                    param.TooltipsForLiveCasino,
                    param.TooltipsForRNGCasino,
                    model.TotalRecords,
                    criteria.PageIndex,
                    criteria.PageSize);
            }

            return new SuperWinLossDetailByPTModel();
        }

        public SuperWinLossDetailMCByPTModel GetSuperWinLossMCDetailByPT(SuperWinLossDetailByPTParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            if (!param.IsValid())
            {
                return new SuperWinLossDetailMCByPTModel();
            }

            var validProductIds = productService.ValidateProductIds(param.GroupName, param.GroupId, Utils.ToEnumerableInt(param.UserSelectedProductIds));
            validProductIds = productDetailService.RemoveProductIds(validProductIds, param.ResourceIds, param.GroupName);
            var productDetailsXML = productDetailService.BuildProductDetailsXML(validProductIds, param.ResourceIds, param.GroupName);

            var criteria = new SuperWinLossDetailMCByPTCriteria(
                param.GroupName,
                param.FromDate,
                param.ToDate,
                reportUtils.BalanceUpDate,
                reportUtils.MoveBetsDate,
                Utils.JoinToString(validProductIds),
                productDetailsXML,
                param.BaseCurrencyName,
                param.Currencies);

            var model = superWinLossMCDetailByPTDataAccess.GetWinLossMCByBetType(criteria);

            if (model.WinLossByProducts.Any())
            {
                var superIds = model.WinLossByProducts.Select(item => item.CustId).Distinct();

                var postionTakings = superWinLossDetailByPTDataAccess.GetSuperPositionTakingList(new SuperPTCriteria(Utils.JoinToString(superIds)));

                var winLossByUsernames = BuildWinLossMCByUsernames(model.WinLossByProducts, postionTakings);

                return new SuperWinLossDetailMCByPTModel(
                    winLossByUsernames,
                    null,
                    param.TooltipsForLiveCasino,
                    param.TooltipsForRNGCasino)
                {
                    TotalByCurrencies = model.TotalByCurrencies
                };
            }

            return new SuperWinLossDetailMCByPTModel();
        }

        private IEnumerable<SuperWinLossDetailByUsernameModel> BuildWinLossByUsernames(
            IEnumerable<SuperWinLossDetailByProductDTO> winLossByProducts,
            IEnumerable<SuperPositionTakingModel> postionTakings)
        {
            var winlossByUsernames = winLossByProducts.Join(
                                postionTakings,
                                winloss => new { winloss.CustId, winloss.ProductId },
                                pt => new { pt.CustId, pt.ProductId },
                                (winloss, pt) =>
                                {
                                    winloss.PositionTaking = pt.PositionTaking;
                                    return winloss;
                                })
                                .GroupBy(winlossDetailByProduct =>
                                    winlossDetailByProduct.CustId, (key, winlossDetailByCustIds) =>
                                        winlossDetailByCustIds.GroupBy(item => item.PositionTaking, (index, winlossGroupByPTs) =>
                                        {
                                            decimal turnover = 0;
                                            decimal netTurnover = 0;
                                            decimal grossComm = 0;
                                            decimal memberWinlost = 0;
                                            decimal memberComm = 0;
                                            decimal memberTotal = 0;
                                            decimal agWinlost = 0;
                                            decimal agComm = 0;
                                            decimal agTotal = 0;
                                            decimal magWinlost = 0;
                                            decimal magComm = 0;
                                            decimal magTotal = 0;
                                            decimal sagWinlost = 0;
                                            decimal sagComm = 0;
                                            decimal sagTotal = 0;
                                            decimal company = 0;

                                            var productNames = new List<string>();

                                            foreach (var winloss in winlossGroupByPTs.OrderBy(winloss => winloss.ProductId))
                                            {
                                                turnover += winloss.Turnover;
                                                netTurnover += winloss.NetTurnover;
                                                grossComm += winloss.GrossComm;

                                                memberWinlost += winloss.MemberWinlost;
                                                memberComm += winloss.MemberComm;
                                                memberTotal += winloss.MemberTotal;

                                                agWinlost += winloss.AgWinlost;
                                                agComm += winloss.AgComm;
                                                agTotal += winloss.AgTotal;

                                                magWinlost += winloss.MAgWinlost;
                                                magComm += winloss.MAgComm;
                                                magTotal += winloss.MAgTotal;

                                                sagWinlost += winloss.SAgWinlost;
                                                sagComm += winloss.SAgComm;
                                                sagTotal += winloss.SAgTotal;
                                                company += winloss.Company;

                                                productNames.Add(CacheRef.GetProductName(winloss.ProductId));
                                            }

                                            return new SuperWinLossDetailByProductDTO(
                                                winlossGroupByPTs.FirstOrDefault().CustId,
                                                winlossGroupByPTs.FirstOrDefault().Username,
                                                winlossGroupByPTs.FirstOrDefault().ProductId,
                                                winlossGroupByPTs.FirstOrDefault().ProductName,
                                                turnover,
                                                netTurnover,
                                                grossComm,
                                                memberWinlost,
                                                memberComm,
                                                memberTotal,
                                                agWinlost,
                                                agComm,
                                                agTotal,
                                                magWinlost,
                                                magComm,
                                                magTotal,
                                                sagWinlost,
                                                sagComm,
                                                sagTotal,
                                                company)
                                            {
                                                PositionTaking = winlossGroupByPTs.FirstOrDefault().PositionTaking,
                                                ProductNames = productNames
                                            };
                                        }));

            return winlossByUsernames.Select(winlossByUsername => new SuperWinLossDetailByUsernameModel(winlossByUsername));
        }

        private IEnumerable<SuperWinLossDetailMCByUsernameModel> BuildWinLossMCByUsernames(
           IEnumerable<SuperWinLossDetailMCByProductDTO> winLossByProducts,
           IEnumerable<SuperPositionTakingModel> postionTakings)
        {
            var winlossByUsernames = winLossByProducts.Join(
                                postionTakings,
                                winloss => new { winloss.CustId, winloss.ProductId },
                                pt => new { pt.CustId, pt.ProductId },
                                (winloss, pt) =>
                                {
                                    winloss.PositionTaking = pt.PositionTaking;
                                    return winloss;
                                })
                                .GroupBy(winlossDetailByProduct =>
                                    winlossDetailByProduct.CustId, (key, winlossDetailByCustIds) =>
                                        winlossDetailByCustIds.GroupBy(item => item.PositionTaking, (index, winlossGroupByPTs) =>
                                        {
                                            decimal turnover = 0;
                                            decimal netTurnover = 0;
                                            decimal grossComm = 0;
                                            decimal memberWinlost = 0;
                                            decimal memberComm = 0;
                                            decimal memberTotal = 0;
                                            decimal agWinlost = 0;
                                            decimal agComm = 0;
                                            decimal agTotal = 0;
                                            decimal magWinlost = 0;
                                            decimal magComm = 0;
                                            decimal magTotal = 0;
                                            decimal sagWinlost = 0;
                                            decimal sagComm = 0;
                                            decimal sagTotal = 0;
                                            decimal company = 0;

                                            var productNames = new List<string>();

                                            foreach (var winloss in winlossGroupByPTs.OrderBy(winloss => winloss.ProductId))
                                            {
                                                turnover += winloss.Turnover;
                                                netTurnover += winloss.NetTurnover;
                                                grossComm += winloss.GrossComm;

                                                memberWinlost += winloss.MemberWinlost;
                                                memberComm += winloss.MemberComm;
                                                memberTotal += winloss.MemberTotal;

                                                agWinlost += winloss.AgWinlost;
                                                agComm += winloss.AgComm;
                                                agTotal += winloss.AgTotal;

                                                magWinlost += winloss.MAgWinlost;
                                                magComm += winloss.MAgComm;
                                                magTotal += winloss.MAgTotal;

                                                sagWinlost += winloss.SAgWinlost;
                                                sagComm += winloss.SAgComm;
                                                sagTotal += winloss.SAgTotal;
                                                company += winloss.Company;

                                                productNames.Add(CacheRef.GetProductName(winloss.ProductId));
                                            }

                                            return new SuperWinLossDetailMCByProductDTO(
                                                winlossGroupByPTs.FirstOrDefault().CustId,
                                                winlossGroupByPTs.FirstOrDefault().Username,
                                                winlossGroupByPTs.FirstOrDefault().CurrencyId,
                                                winlossGroupByPTs.FirstOrDefault().CurrencyName,
                                                winlossGroupByPTs.FirstOrDefault().ProductId,
                                                winlossGroupByPTs.FirstOrDefault().ProductName,
                                                turnover,
                                                netTurnover,
                                                grossComm,
                                                memberWinlost,
                                                memberComm,
                                                memberTotal,
                                                agWinlost,
                                                agComm,
                                                agTotal,
                                                magWinlost,
                                                magComm,
                                                magTotal,
                                                sagWinlost,
                                                sagComm,
                                                sagTotal,
                                                company)
                                            {
                                                PositionTaking = winlossGroupByPTs.FirstOrDefault().PositionTaking,
                                                ProductNames = productNames
                                            };
                                        }));

            return winlossByUsernames.Select(winlossByUsername => new SuperWinLossDetailMCByUsernameModel(winlossByUsername));
        }
    }
}