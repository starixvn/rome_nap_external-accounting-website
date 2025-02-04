namespace Fanex.ExternalAccount.DataAccess.Criteria.SuperWinLossDetailByPT
{
    using System;

    public class SuperWinLossDetailMCByPTCriteria : SuperWinLossDetailByPTCriteria
    {
        public SuperWinLossDetailMCByPTCriteria(
            string groupName,
            DateTime fromDate,
            DateTime toDate,
            DateTime balanceUpDay,
            DateTime moveBetsDay, 
            string productIds, 
            string productDetails,
            string baseCurrencyName,
            string currencies) : base(groupName, fromDate, toDate, balanceUpDay, moveBetsDay, productIds, productDetails, 0, 0)
        {
            BaseCurrency = baseCurrencyName;
            Currencies = currencies;
        }
        
        public string BaseCurrency { get; }

        public string Currencies { get; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_ByPT";
    }
}
