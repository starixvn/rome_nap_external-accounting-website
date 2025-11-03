namespace Fanex.ExternalAccount.HeadlineList.Models
{
    public class HeadlineEntity
    {
        public int GroupId { get; set; }

        public string GroupName { get; set; }
        
        public int CustId { get; set; }
        
        public string Username { get; set; }
        
       // public string Status { get; set; }
        
        public bool IsEnable { get; set; }
        
        public bool IsSuspend { get; set; }
        
        public bool IsClosed { get; set; }
        
        public byte ClosedReason { get; set; }
        
        public string Status => !IsEnable ? "Disabled" : IsClosed ? "Closed" : IsSuspend ? "Suspended" : "Open";
    }
}