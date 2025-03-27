using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetMasterWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MasterId { get; set; }
        
        public string ProductDetailsXML { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_SAg";
    }
}
