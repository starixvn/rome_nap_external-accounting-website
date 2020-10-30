namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DTO.Report;
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

        public SuperWinLossDetailByPTServiceImpl(SuperWinLossDetailByPTDataAccess superWinLossDetailByPTDataAccess)
        {
            this.superWinLossDetailByPTDataAccess = superWinLossDetailByPTDataAccess ?? new SuperWinLossDetailByPTDataAccessImpl();
        }

        public SuperWinLossDetailByPTServiceImpl() : this(null)
        {
        }

        public SuperWinLossDetailByPTModel GetSuperWinLossDetailByPT(SuperWinLossDetailByPTParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            //var criteria = new SuperWinLossDetailByPTCriteria(
            //    param.CustId,
            //    param.FromDate,
            //    param.ToDate,
            //    DateTime.Now,//reportDateService.GetBalanceUpDay(),
            //    DateTime.Now,//reportDateService.GetMoveBetsDay(),
            //    param.UserSelectedProductIds,
            //    //param.CustLevel,
            //    param.ProductDetailsXML);

            var criteria = new SuperWinLossDetailByPTCriteria(
                "ACCINDO",
               new DateTime(2020, 10, 04),
               new DateTime(2020, 10, 12),
               new DateTime(2020, 9, 30),//reportDateService.GetBalanceUpDay(),
               new DateTime(2020, 10, 03),//reportDateService.GetMoveBetsDay(),
              "1,3,28,8,40,38,5,24,42,21,47,12,39,45,52,22,6,34,36,32,55,56",
              "All",
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

                                foreach (var winloss in winlossGroupByPTs)
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
                    model.TotalRecords,
                    criteria.PageIndex,
                    criteria.PageSize);
            }

            return new SuperWinLossDetailByPTModel();
        }
    }
}