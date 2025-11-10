using System;
using System.Collections.Generic;

namespace Fanex.ExternalAccount.ReportsAndStatements.Models.ViewModels
{
    public class StatementReportModel
    {
        public int CustId { get; set; }
        public string Username { get; set; }
        public string GroupName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public List<StatementDetail> Details { get; set; } = new List<StatementDetail>();
    }

    public class StatementDetail
    {
        public string Date { get; set; }
        public string Remark { get; set; }
        public decimal Amount { get; set; }
        public string Type { get; set; }
    }
}
