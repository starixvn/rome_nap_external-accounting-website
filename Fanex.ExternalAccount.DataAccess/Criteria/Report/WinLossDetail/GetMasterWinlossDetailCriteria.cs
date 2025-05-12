using System;
using Fanex.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetMasterWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public int? MasterId { get; set; }

        public int? MemberId { get; set; }

        [SpParam(SqlParamName = "ProductDetails")]
        public string ProductDetailsXML { get; set; }
        
        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_SAg";
    }
}
