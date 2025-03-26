namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    public class GetDetailReportDataCriteria : GetSummaryReportDataCriteria
    {
        public override bool IsValid() => 
            !string.IsNullOrEmpty(GroupName) && 
            !string.IsNullOrWhiteSpace(FromDate);

        public string BetTypes { get; set; }

        public string MatchIds { get; set; }

        public override string GetSettingKey() => "EAS_MatchWinloss_Detail";
    }
}