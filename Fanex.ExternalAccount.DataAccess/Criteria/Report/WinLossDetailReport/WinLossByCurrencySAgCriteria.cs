using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class WinLossByCurrencySAgCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MasterId { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_SAg";
    }
} 