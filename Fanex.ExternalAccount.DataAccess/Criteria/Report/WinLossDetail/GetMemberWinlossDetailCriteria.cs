namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    using System;
    using Fanex.Data;

    public class GetMemberWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MemberId { get; set; }

        [SpParam(SqlParamName = "ProductDetails")]
        public string ProductDetailsXML { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_Ag";
    }
}
