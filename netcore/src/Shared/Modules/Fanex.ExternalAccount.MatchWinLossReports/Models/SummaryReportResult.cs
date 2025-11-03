namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using System.Collections.Generic;
    public class SummaryReportResult
    {
        public List<ReportRawEntity> Events { get; set; }
        
        public List<ReportRawEntity> GroupLeagues { get; set; }
        
        public ReportRawEntity Total { get; set; }
        
        public ReportRawEntity GrandTotal { get; set; }
        
        public List<ReportRawEntity> SystemParlayOutright { get; set; }
        
        public int TotalItems { get; set; }
    }
}