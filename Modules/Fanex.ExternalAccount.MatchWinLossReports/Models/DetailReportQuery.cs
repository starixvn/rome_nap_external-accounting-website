namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    public class DetailReportQuery : SummaryReportQuery
    {
        public string Matches { get; set; }
        
        public string BetTypes { get; set; }
    }
}