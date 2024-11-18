namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using System;

    public class SuperWinLossDetailMCByPTCriteria : SuperWinLossDetailByPTCriteria
    {
        private readonly string[] mappingBaseCurrency = { "MYR", "CNY", "THB", "IDR" };
        public SuperWinLossDetailMCByPTCriteria(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay, 
            string productIds, 
            string productDetails,
            int baseCurrency,
            string currencyIds) : base(groupName, fromDate, toDate, balanceUpDay, moveBetsDay, productIds, productDetails, 0, 0)
        {
            BaseCurrency = mappingBaseCurrency[baseCurrency-1];
            CurrencyIds = currencyIds;
        }
        
        public string BaseCurrency { get; }

        public string CurrencyIds { get; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_ByPT";
    }
}
