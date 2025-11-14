namespace Fanex.ExternalAccount.RewardManagement.Requests
{
    using System;

    public class RewardReportRequest
    {
        public DateTime FromDate { get; set; } = DateTime.Today;

        public DateTime ToDate { get; set; } = DateTime.Today;

        public string ProgramIds { get; set; }

        public string Username { get; set; }

        public string GroupName { get; set; }
        
        public byte? MemberPaymentStatus { get; set; }
        
        public int PageSize { get; set; }
        
        public int Page { get; set; }
    }
}