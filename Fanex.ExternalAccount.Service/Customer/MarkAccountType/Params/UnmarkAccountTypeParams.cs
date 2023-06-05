namespace Fanex.ExternalAccount.Service.Customer.MarkAccountType.Params
{
    using System.Collections;
    using System.Collections.Generic;
    
    public class UnmarkAccountTypeParams
    {
        public IEnumerable<int> CustIds { get; set; }

        public int CustId { get; set; }

        public int UserId { get; set; }

        public string IP { get; set; }
    }
}
