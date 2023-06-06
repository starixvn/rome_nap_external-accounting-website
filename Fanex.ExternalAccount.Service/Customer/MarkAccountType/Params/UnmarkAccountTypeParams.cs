namespace Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params
{
    using Fanex.ExternalAccount.DTO.Customer.MarkAccount;
    using System.Collections;
    using System.Collections.Generic;
    
    public class UnmarkAccountTypeParams
    {
        public IEnumerable<MarkAccount> Accounts { get; set; }

        public int CustId { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }
    }
}
