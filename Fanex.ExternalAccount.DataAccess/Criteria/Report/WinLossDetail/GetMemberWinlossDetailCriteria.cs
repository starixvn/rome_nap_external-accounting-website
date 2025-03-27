namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using System;

    public class GetMemberWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MemberId { get; set; }
        
        public string ProductDetailsXML { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_Ag";
    }
}
