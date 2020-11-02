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

    public interface SuperWinLossDetailByPTService
    {
        SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param);
    }

    public class SuperWinLossDetailByPTServiceImpl : SuperWinLossDetailByPTService
    {
        private readonly SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess;
        private readonly IReportService reportService;

        public SuperWinLossDetailByPTServiceImpl(
            SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess,
            IReportService reportService)
        {
            this.superWinLossDetailByPTDataAccess = superWinLossDetailByPTDataAccess ?? new SuperWinLossDetailByPTDataAccessImpl();
            this.reportService = reportService ?? new ReportService();
        }

        public SuperWinLossDetailByPTServiceImpl() : this(null, null)
        {
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
              param.ResourceIds,
              param.PageSize,
              param.PageIndex);

            var model = superWinLossDetailByPTDataAccess.GetWinLossByBetType(criteria);

            if (model.WinLossByProducts.Any())
            {
                var superIds = model.WinLossByProducts.Select(item => item.CustId).Distinct();
                var postionTakings = superWinLossDetailByPTDataAccess.GetSuperPositionTakingList(new SuperPTCriteria(string.Join(",", superIds)));

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
                                var winlossbyPTTotal = new SuperWinLossDetailByProductDTO
                                {
                                    ProductNames = new List<string>()
                                };

                                foreach (var winloss in winlossGroupByPTs.OrderBy(winloss => winloss.ProductName))
                                {
                                    winlossbyPTTotal.CustId = winloss.CustId;
                                    winlossbyPTTotal.Username = winloss.Username;
                                    winlossbyPTTotal.PositionTaking = winloss.PositionTaking;

                                    winlossbyPTTotal.Turnover += winloss.Turnover;
                                    winlossbyPTTotal.NetTurnover += winloss.NetTurnover;
                                    winlossbyPTTotal.Turnover += winloss.Turnover;
                                    winlossbyPTTotal.GrossComm += winloss.GrossComm;

                                    winlossbyPTTotal.MemberWinlost += winloss.MemberWinlost;
                                    winlossbyPTTotal.PlayerComm += winloss.PlayerComm;
                                    winlossbyPTTotal.MemberTotal += winloss.MemberTotal;

                                    winlossbyPTTotal.AgWinlost += winloss.AgWinlost;
                                    winlossbyPTTotal.AgComm += winloss.AgComm;
                                    winlossbyPTTotal.AgTotal += winloss.AgTotal;

                                    winlossbyPTTotal.MAgWinlost += winloss.MAgWinlost;
                                    winlossbyPTTotal.MAgComm += winloss.MAgWinlost;
                                    winlossbyPTTotal.MAgTotal += winloss.MAgComm;

                                    winlossbyPTTotal.SAgWinlost += winloss.SAgWinlost;
                                    winlossbyPTTotal.SAgComm += winloss.SAgComm;
                                    winlossbyPTTotal.SAgTotal += winloss.SAgTotal;

                                    winlossbyPTTotal.Company += winloss.Company;

                                    winlossbyPTTotal.ProductNames.Add(winloss.ProductName);
                                }

                                return winlossbyPTTotal;
                            }));

                return new SuperWinLossDetailByPTModel(
                    winlossByUsernames.Select(winlossByUsername => new SuperWinLossDetailByUsernameModel(winlossByUsername)),
                    model.Total,
                    param.TooltipsForLiveCasino,
                    model.TotalRecords,
                    criteria.PageIndex,
                    criteria.PageSize);
            }

            return new SuperWinLossDetailByPTModel();
        }
    }
}