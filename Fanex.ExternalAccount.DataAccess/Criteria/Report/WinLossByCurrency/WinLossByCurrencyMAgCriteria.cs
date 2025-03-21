using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class WinLossByCurrencyMAgCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? AgentId { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetailByCurrency_MAg";
    }
} 