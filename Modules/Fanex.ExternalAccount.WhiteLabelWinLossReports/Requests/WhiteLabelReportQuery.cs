namespace Fanex.ExternalAccount.WhiteLabelWinLossReports.Requests
{
    using System;
    using System.Web.Mvc;
    public class WhiteLabelReportQuery
    {
        public DateTime FromDate { get; set; } = DateTime.Today;

        public DateTime ToDate { get; set; }  = DateTime.Today;
        
        public string WhiteLabels { get; set; }

        [AllowHtml]
        public string Username { get; set; }

        public string GroupName { get; set; } 
    }
}