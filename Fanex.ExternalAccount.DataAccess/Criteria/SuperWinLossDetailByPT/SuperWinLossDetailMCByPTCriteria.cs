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
            int baseCurrency,
            string currencyIds) : base(groupName, fromDate, toDate, balanceUpDay, moveBetsDay, productIds, productDetails, 0, 0)
        {
            BaseCurrency = baseCurrency;
            CurrencyIds = currencyIds;
        }
        
        public int BaseCurrency { get; }

        public string CurrencyIds { get; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_ByPT";
    }
}
