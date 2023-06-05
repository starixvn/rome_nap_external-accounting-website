namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    using System.Collections.Generic;
    
    public class UnmarkAccountRequest
    {
        public IEnumerable<int> CustIds { get; set; }
        public int CustId { get; set; }
    }
}
