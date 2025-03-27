using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class WinLossByCurrencyComCriteria : BaseWinLossByCurrencyCriteria
    {
        public string Currencies { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_Com";

        public override bool IsValid() => true;
    }
} 