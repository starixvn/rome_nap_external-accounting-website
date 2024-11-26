namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using System.Collections.Generic;
    
    public class DetailReportResult : SummaryReportResult
    {
     
        public List<ReportRawEntity> SubTotals { get; set; } = new List<ReportRawEntity>();
        
        public List<ReportRawEntity> Mains { get; set; } = new List<ReportRawEntity>();
        
    }
}