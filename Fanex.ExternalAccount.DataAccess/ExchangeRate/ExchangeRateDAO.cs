namespace Fanex.ExternalAccount.DataAccess.ExchangeRate
{
    using Fanex.Common;
    using Fanex.Data;
    using Fanex.ExternalAccount.DataAccess.Criteria.ExchangeRate;
    using Fanex.ExternalAccount.DTO.Report;
    using System.Collections.Generic;
    using Utilities;


    public interface IExchangeRateDAO
    {
        IEnumerable<ExchangeRateModel> GetOandaExchangeRate(SelectOandaExchangeRateCriteria criteria);
    }

    public class ExchangeRateDAO : IExchangeRateDAO
    {
        public IEnumerable<ExchangeRateModel> GetOandaExchangeRate(SelectOandaExchangeRateCriteria criteria)
        {
            using (ObjectDb objDb = new ObjectDb(criteria.GetSettingKey()))
            {
                objDb.AssignParamValues(criteria.GetSpParams());
                var reader = objDb.ExecuteReader();
                while (reader.Read())
                {
                    yield return new ExchangeRateModel
                    {
                        CurrencyCode = Converter.ToString2(reader["CurrencyCode"]),
                        CurrencyName = Converter.ToString2(reader["CurrencyName"]),
                        CurrencyId = Converter.ToInt(reader["CurrencyId"]),
                        ExchangeDate = Converter.ToDateTime(reader.ToString("ExchangeDate")),
                        MYRRate = Converter.ToDecimal(reader["MYR/Unit"]),
                        CNYRate = Converter.ToDecimal(reader["CNY/Unit"]),
                        THBRate = Converter.ToDecimal(reader["THB/Unit"]),
                        IDRRate = Converter.ToDecimal(reader["IDR/Unit"])
                    };
                }
            }
        }
    }
}
