using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class WinLossByCurrencyAgCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MemberId { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_Ag";
    }
} 