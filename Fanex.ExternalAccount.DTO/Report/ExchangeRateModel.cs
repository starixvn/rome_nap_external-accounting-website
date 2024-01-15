namespace Fanex.ExternalAccount.DTO.Report
{
    using System;
    using System.Collections.Generic;

    public class ExchangeRateModel
    {
        public string CurrencyName { get; set; }

        public string CurrencyCode { get; set; }

        public int CurrencyId { get; set; }

        public decimal MYRRate { get; set; }

        public decimal CNYRate { get; set; }

        public decimal THBRate { get; set; }

        public decimal IDRRate { get; set; }

        public DateTime ExchangeDate { get; set; }

        public string ExchangeDateString => ExchangeDate.ToString("MM/dd/yyyy");
    }

    public class ExchangeRateByDateModel
    {
        public string ExchangeDateString { get; set; }

        public IEnumerable<ExchangeRateModel> Rates { get; set; }
    }
}
