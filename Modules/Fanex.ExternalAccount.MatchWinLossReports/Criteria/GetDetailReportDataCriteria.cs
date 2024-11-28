namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    public class GetDetailReportDataCriteria : GetSummaryReportDataCriteria
    {
        public override bool IsValid()
        {
            return base.IsValid() && !string.IsNullOrWhiteSpace(BetTypes);
        }

        public string BetTypes { get; set; }
        
        public string MatchIds { get; set; }

        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_Detail";
        }
    }
}