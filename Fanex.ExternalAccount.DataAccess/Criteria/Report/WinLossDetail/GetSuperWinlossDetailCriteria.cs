using System;
using Fanex.Data;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetSuperWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        [SpParam(SqlParamName = "ProductDetails")]
        public string ProductDetailsXML { get; set; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_Com";
    }
}
