namespace Fanex.ExternalAccount.Service.Report
{
    using Fanex.ExternalAccount.DataAccess.Criteria.ExchangeRate;
    using Fanex.ExternalAccount.DataAccess.ExchangeRate;
    using Fanex.ExternalAccount.DTO.Report;
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public interface IExchangeService
    {
        IEnumerable<ExchangeRateByDateModel> GetOandaExchangeRate(DateTime fromDate, DateTime toDate, string groupName);
    }

    public class ExchangeService : IExchangeService
    {
        private readonly IExchangeRateDAO exchangeRateDAO;

        public ExchangeService(IExchangeRateDAO exchangeRateDAO = null)
        {
            this.exchangeRateDAO = exchangeRateDAO ?? new ExchangeRateDAO();
        }
        public IEnumerable<ExchangeRateByDateModel> GetOandaExchangeRate(DateTime fromDate, DateTime toDate, string groupName)
        {
            return exchangeRateDAO.GetOandaExchangeRate(new SelectOandaExchangeRateCriteria
            {
                GroupName = groupName,
                FromDate = fromDate,
                ToDate = toDate
            }).GroupBy(exchangeRate => exchangeRate.ExchangeDateString).Select(group => new ExchangeRateByDateModel { ExchangeDateString = group.Key, Rates = group.ToList()});
        }
    }
}
