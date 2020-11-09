namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DTO.Report;
    using Fanex.ExternalAccount.Service.Impl.Report;
    using Fanex.ExternalAccount.Service.Interface.Report;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using CasinoLibrary.SBAgent;
    using System.Data;
    using Fanex.ExternalAccount.Core.Shared;
    using Fanex.ExternalAccount.Service.Products;

    public interface SuperWinLossDetailByPTService
    {
        SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param);
    }

    public class SuperWinLossDetailByPTServiceImpl : SuperWinLossDetailByPTService
    {
        private readonly SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess;
        private readonly IProductDetailService productDetailService;

        public SuperWinLossDetailByPTServiceImpl(
            SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess = null,
            IProductDetailService productDetailService = null)
        {
            this.superWinLossDetailByPTDataAccess = superWinLossDetailByPTDataAccess ?? new SuperWinLossDetailByPTDataAccessImpl();
            this.productDetailService = productDetailService ?? new ProductDetailService();
        }

        public SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var criteria = new SuperWinLossDetailByPTCriteria(
                param.GroupName,
                param.FromDate,
                param.ToDate,
                ReportService.BalanceUpDate,
                ReportService.MoveBetsDate,
                param.UserSelectedProductIds,
                productDetailService.BuildProductDetailsXML(param.UserSelectedProductIds, param.ResourceIds, param.GroupName),
                param.PageSize,
                param.PageIndex);

            var model = superWinLossDetailByPTDataAccess.GetWinLossByBetType(criteria);

            if (model.WinLossByProducts.Any())
            {
                var superIds = model.WinLossByProducts.Select(item => item.CustId).Distinct();
                var superPositionTakingByRnG = GetSuperPositionTakingByRnG(superIds);

                var postionTakings = superWinLossDetailByPTDataAccess
                    .GetSuperPositionTakingList(new SuperPTCriteria(string.Join(",", superIds)))
                    .Concat(superPositionTakingByRnG);

                var winlossByUsernames = model.WinLossByProducts.Join(
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

                                foreach (var winloss in winlossGroupByPTs.OrderBy(winloss => winloss.ProductName))
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
                                    magComm += winloss.MAgWinlost;
                                    magTotal += winloss.MAgComm;

                                    sagWinlost += winloss.SAgWinlost;
                                    sagComm += winloss.SAgComm;
                                    sagTotal += winloss.SAgTotal;
                                    company += winloss.Company;

                                    productNames.Add(winloss.ProductName);
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

                return new SuperWinLossDetailByPTModel(
                    winlossByUsernames.Select(winlossByUsername => new SuperWinLossDetailByUsernameModel(winlossByUsername)),
                    model.GrandTotal,
                    param.TooltipsForLiveCasino,
                    model.TotalRecords,
                    criteria.PageIndex,
                    criteria.PageSize);
            }

            return new SuperWinLossDetailByPTModel();
        }

        internal IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingByRnG(IEnumerable<int> superIds)
        {
            var customer = new Customer();
            var rngCasinoPositionTakingsTable = customer.GetSuperDesktopPT(superIds.Select(id => id.ToString()).ToArray());
            var rngCasinoPTs = new List<SuperPositionTakingModel>();

            foreach (DataRow row in rngCasinoPositionTakingsTable.Rows)
            {
                rngCasinoPTs.Add(new SuperPositionTakingModel(
                    Fanex.Common.Converter.ToInt(row["OBSuperID"]),
                    Fanex.Common.Converter.ToDecimal(row["SPositionTaking"]),
                    ProductConstants.RNGCasino,
                    ""));
            }

            return rngCasinoPTs;
        }
    }
}