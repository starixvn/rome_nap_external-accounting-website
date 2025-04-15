using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetAgentWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? AgentId { get; set; }

        public int? MemberId { get; set; }

        public string ProductDetailsXML { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_MAg";
    }
}
