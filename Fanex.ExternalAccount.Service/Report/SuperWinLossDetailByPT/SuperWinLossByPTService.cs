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
              "All");

            var superWinLossDetailByProductModels = superWinLossDetailByPTDataAccess.GetWinLossByBetType(criteria);

            if (superWinLossDetailByProductModels.Any())
            {
                var winLossDetailByPTAfterGroup = superWinLossDetailByProductModels
                .Select(product =>
                {
                    var superIds = superWinLossDetailByProductModels.Select(item => item.CustId).Distinct();
                    var postionTakings = superWinLossDetailByPTDataAccess.GetSuperPositionTakingList(new SuperPTCriteria(string.Join(",", superIds)));
                    var existedPT = postionTakings.Where(pt => pt.CustId.Equals(product.CustId) && pt.ProductID.Equals(product.ProductID));
                    
                    if (existedPT.Any())
                    {
                        product.PositionTaking = existedPT.FirstOrDefault().PositionTaking;
                    }

                    return product;
                })
                .GroupBy(item => item.CustId)
                .Select(ss => ss.GroupBy(item => item.PositionTaking).Select(group =>
                {
                    var result = new SuperWinLossDetailByProductModel
                    {
                        ProductNames = new List<string>()
                    };

                    foreach (var product in group)
                    {
                        result.CustId = product.CustId;
                        result.Username = product.Username;
                        result.PositionTaking += product.PositionTaking;
                        result.Turnover += product.Turnover;
                        result.NetTurnover += product.NetTurnover;
                        result.Turnover += product.Turnover;
                        result.GrossComm += product.GrossComm;
                        result.MemberWinlost += product.MemberWinlost;
                        result.PlayerComm += product.PlayerComm;
                        result.MemberTotal += product.MemberTotal;
                        result.AgWinlost += product.AgWinlost;
                        result.AgComm += product.AgComm;
                        result.AgTotal += product.AgTotal;
                        result.ProductNames.Add(product.ProductName);
                    }

                    return result;
                }));

                return new SuperWinLossDetailByPTModel(
                winLossDetailByPTAfterGroup.Select(
                item => new SuperWinLossDetailProductByUsername(
                item.FirstOrDefault().CustId,
                item.FirstOrDefault().Username,
                item.OrderBy(superWinLossDetailByPTModel => superWinLossDetailByPTModel.PositionTaking))));
            }

            return new SuperWinLossDetailByPTModel();
        }
    }
}