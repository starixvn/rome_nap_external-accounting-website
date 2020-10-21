namespace Fanex.ExternalAccount.Service.Report.SuperWinLossDetailByPT
{
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DataAccess.Impl.Report;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
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
                var superWinLossDetailProductByUsernames = superWinLossDetailByProductModels.GroupBy(item => item.CustId);

                return new SuperWinLossDetailByPTModel(
                superWinLossDetailProductByUsernames.Select(
                item => new SuperWinLossDetailProductByUsername(item.FirstOrDefault().CustId, item.FirstOrDefault().Username, item)));
            }

            return new SuperWinLossDetailByPTModel();
        }
    }
}