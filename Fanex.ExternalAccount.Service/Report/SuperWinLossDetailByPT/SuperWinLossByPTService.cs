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
        IEnumerable<SuperWinLossDetailByPTModel> GetWinLossByProduct(SuperWinLossDetailByPTParam param);
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

        public IEnumerable<SuperWinLossDetailByPTModel> GetWinLossByProduct(SuperWinLossDetailByPTParam param)
        {
            if (param == null)
            {
                throw new ArgumentNullException(nameof(param));
            }

            var criteria = new SuperWinLossDetailByPTCriteria(
                param.CustId,
                param.FromDate,
                param.ToDate,
                DateTime.Now,//reportDateService.GetBalanceUpDay(),
                DateTime.Now,//reportDateService.GetMoveBetsDay(),
                param.UserSelectedProductIds,
                //param.CustLevel,
                param.ProductDetailsXML);

            var winLossByBetTypeData = superWinLossDetailByPTDataAccess.GetWinLossByBetType(criteria);
            var result = new List<SuperWinLossDetailByPTModel>();

            if (winLossByBetTypeData.Any())
            {
                var winLossByProducts = winLossByBetTypeData.GroupBy(winLossByBetType => winLossByBetType.ProductId);

                foreach (var winLossByProduct in winLossByProducts)
                {
                    result.Add(new SuperWinLossDetailByPTModel(
                        winLossByProduct.Key,
                        "Product Name",//cacheRefService.GetProductName(winLossByProduct.Key),
                        CreateWinLossBySportModel(winLossByProduct)));
                }
            }

            return result;
        }

        private static IEnumerable<SuperWinLossDetailSportByPTModel> CreateWinLossBySportModel(
            IGrouping<int, WinLossByBetTypeModel> winLossByProduct)
        {
            var winLossBySports = winLossByProduct.GroupBy(winLossByBetType => new
            {
                winLossByBetType.SportTypeId,
                winLossByBetType.SportTypeName
            });

            var result = new List<SuperWinLossDetailSportByPTModel>();

            foreach (var winLossBySport in winLossBySports)
            {
                result.Add(new SuperWinLossDetailSportByPTModel(
                    winLossBySport.Key.SportTypeId,
                    winLossBySport.Key.SportTypeName,
                    winLossBySport.ToList()));
            }

            return result;
        }
    }
}