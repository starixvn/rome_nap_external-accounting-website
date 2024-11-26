namespace Fanex.ExternalAccount.MatchWinLossReports.Models
{
    using System;
    
    public class SummaryReportQuery
    {
        public int PageSize { get; set; } = 500;
        
        public int Page { get; set; } = 1;
        
        public DateTime FromDate { get; set; } = DateTime.Today;

        public string FromDateString => FromDate.ToString("MM/dd/yyyy");
        
        public DateTime ToDate { get; set; }  = DateTime.Today;
        
        public string ToDateString => ToDate.ToString("MM/dd/yyyy");
        
        public int Sport { get; set; }
        
        public string Leagues { get; set; }
        
        public string Currencies { get; set; }
        
        public string BaseCurrency { get; set; }
        
        public string GroupName { get; set; } 
        
        public string SortBy { get; set; }

        public byte SortType { get; set; }

        public bool IsMY { get; set; }
    }
}