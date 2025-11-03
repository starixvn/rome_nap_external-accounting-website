namespace Fanex.ExternalAccount.ViewModel.SubGroup
{
    using System.Collections.Generic;
    using Fanex.ExternalAccount.DTO.SubGroup;

    public class SubGroupViewModel
    {
        public IEnumerable<SubGroupCustomer> SubGroupCustomers { get; set; }

        public IEnumerable<string> SubGroups { get; set; }

        public int PageIndex { get; set; }

        public int TotalItems { get; set; }
    }
}
