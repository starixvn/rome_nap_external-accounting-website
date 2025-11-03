namespace Fanex.ExternalAccount.ViewModel.Customer.MarkAccountType
{
    using Fanex.ExternalAccount.DTO.Customer.MarkAccount;
    using System.Collections.Generic;

    public class MarkAccountViewModel
    {
        public MarkAccountViewModel()
        {
            Customers = new List<MarkAccount>();
        }

        public IEnumerable<MarkAccount> Customers { get; set; }

        public int PageSize { get; set; }

        public int PageIndex { get; set; }

        public int TotalRows { get; set; }
    }
}
