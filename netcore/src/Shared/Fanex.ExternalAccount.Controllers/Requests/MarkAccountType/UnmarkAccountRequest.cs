namespace Fanex.ExternalAccount.Controllers.Requests.MarkAccountType
{
    using Fanex.ExternalAccount.DTO.Customer.MarkAccount;
    using System.Collections.Generic;
    
    public class UnmarkAccountRequest
    {
        public IEnumerable<MarkAccount> Accounts { get; set; }

        public int CustId { get; set; }
    }
}
