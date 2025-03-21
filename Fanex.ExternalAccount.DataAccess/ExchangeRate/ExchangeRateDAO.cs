namespace Fanex.ExternalAccount.DataAccess.ExchangeRate
{
    using System.Collections.Generic;
    using Fanex.Data.Repository;
    using Fanex.ExternalAccount.DataAccess.Criteria.ExchangeRate;
    using Fanex.ExternalAccount.DTO.Report;


    public interface IExchangeRateDAO
    {
        IEnumerable<ExchangeRateModel> GetOandaExchangeRate(SelectOandaExchangeRateCriteria criteria);
    }

    public class ExchangeRateDAO : IExchangeRateDAO
    {

        private readonly IDynamicRepository _dynamicRepository;

        public ExchangeRateDAO() : this(null)
        {
        }

        public ExchangeRateDAO(IDynamicRepository dynamicRepository)
        {
            _dynamicRepository = dynamicRepository ?? new DynamicRepository();
        }

        public IEnumerable<ExchangeRateModel> GetOandaExchangeRate(SelectOandaExchangeRateCriteria criteria)
            => _dynamicRepository.Fetch<ExchangeRateModel>(criteria);
    }
}
