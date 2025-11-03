namespace Fanex.ExternalAccount.Controllers.ViewModels.CustomerList
{
    using Fanex.ExternalAccount.DTO.Customer.CustomerList;
    using System.Collections.Generic;
    using System.Linq;

    public class SearchCustomerListViewModel
    {
        public IEnumerable<CustomerListDTO> CustomerList { get; set; }

        public int PageIndex { get; set; }

        public int TotalRows { get; set; }

        public bool HasSubGroups { get; set; }

        public SearchCustomerListViewModel()
        {
            CustomerList = new List<CustomerListDTO>();
        }
    }
}
