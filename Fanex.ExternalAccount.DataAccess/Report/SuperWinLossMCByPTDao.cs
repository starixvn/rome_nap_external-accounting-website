namespace Fanex.ExternalAccount.DataAccess.Impl.Report
{
    using Fanex.Common;
    using Fanex.Common.Extensions;
    using Fanex.Data;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;
    using System.Data;

    public interface SuperWinLossMCDetailByPTDao
    {
        SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria);

        IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria);
    }

    public class SuperWinLossMCDetailByPTDaoImpl : SuperWinLossMCDetailByPTDao
    {
        private readonly IDynamicRepository dynamicRepository;

        public SuperWinLossMCDetailByPTDaoImpl() : this(null)
        {
        }

        public SuperWinLossMCDetailByPTDaoImpl(IDynamicRepository dynamicRepository)
        {
            this.dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<SuperPositionTakingModel> GetSuperPositionTakingList(SuperPTCriteria criteria)
        {
            if (criteria == null)
            {
                throw new ArgumentNullException(nameof(criteria), "Criteria cannot be null");
            }

            return dynamicRepository.Fetch<SuperPositionTakingModel>(criteria);
        }

        public SuperWinLossDetailMCByProductModel GetWinLossMCByBetType(SuperWinLossDetailMCByPTCriteria criteria)
        {
            var dbResult = dynamicRepository.FetchMultiple<SuperWinLossDetailMCByProductDTO, SuperWinLossDetailMCByProductDTO>(criteria);
            return new SuperWinLossDetailMCByProductModel(dbResult.Item1, dbResult.Item2);
        }
    }
}