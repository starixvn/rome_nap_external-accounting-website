namespace Fanex.ExternalAccount.MatchWinLossReports.Criteria
{
    //using Fanex.NAP.Framework.Data;
    using Fanex.Data.Repository;
    public class GetSummaryReportDataCriteria : CriteriaBase
    {
        public string FromDate { get; set; }

        public string ToDate { get; set; }

        public string GroupName { get; set; }

        public string LeagueIds { get; set; }

        public string Currencies { get; set; }

        public string BaseCurrency { get; set; }

        public int SportType { get; set; }

        public string SortBy { get; set; }

        public byte SortType { get; set; }

        public int Page { get; set; } = 1;

        public int PageSize { get; set; } = 500;
        
        public int TotalRows { get; set; }

        public override bool IsValid() =>
            !string.IsNullOrEmpty(GroupName) &&
            !string.IsNullOrWhiteSpace(LeagueIds) &&
            !string.IsNullOrWhiteSpace(FromDate) &&
            !string.IsNullOrWhiteSpace(ToDate);

        public override string GetSettingKey()
        {
            return "EAS_MatchWinloss_Summary";
        }
    }
}