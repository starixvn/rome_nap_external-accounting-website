using System;

namespace Fanex.ExternalAccount.DataAccess.Criteria.Report
{
    public class GetSuperWinlossDetailCriteria : BaseWinLossByCurrencyCriteria
    {
        public string ProductDetailsXML { get; set; }

        public override string GetSettingKey() => "EAS_Rpt_WinLossDetail_Com";
    }
}
